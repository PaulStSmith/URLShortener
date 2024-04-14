using NHibernate;
using NHibernate.Exceptions;
using Npgsql;
using System.Text.Json;
using URLShortener.Common.Model;
using URLShortener.Messages;
using URLShortener.Model;
using URLShortener.Repositories;

namespace URLShortener
{
    /// <summary>
    /// Main class and entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the <see cref="NHibernate.ISessionFactory"/>.
        /// </summary>
        public static ISessionFactory SessionFactory { get; private set; }

        /// <summary>
        /// Gets or sets the current <see cref="WebApplication"/>.
        /// </summary>
        public static WebApplication App { get; private set; }

        /// <summary>
        /// Gets the <see cref="ShortUrlRepository"/> for the application.
        /// </summary>
        public static ShortUrlRepository ShortUrlRepository { get; private set; } = new ShortUrlRepository();

        /// <summary>
        /// Gets the base for the short url.
        /// </summary>
        internal static string ShortUrlBase
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_ShorUrlBase))
                    _ShorUrlBase = Program.App.Configuration.GetValue<string>("shorUrlBase") ?? "";

                return _ShorUrlBase;
            }
        }
        private static string _ShorUrlBase = "";

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebApplication(args);
            ConfigureApplication();
            ConfigureNHibernate();

            // Load url.json
            LoadUrlJson();

            // Start the application
            App.Run();
        }

        /// <summary>
        /// Configures the NHibernate, based on the �appsettings.json� file.
        /// </summary>
        private static void ConfigureNHibernate()
        {
            // Configure NHibernate
            var cfg = new NHibernate.Cfg.Configuration();
            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, App.Configuration["NHibernate:dialect"]);
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, App.Configuration["NHibernate:driver"]);
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, App.Configuration.GetConnectionString("db"));
            using (var str = new FileStream("NHibernate.map.xml", FileMode.Open))
                cfg.AddInputStream(str);
            SessionFactory = cfg.BuildSessionFactory();
        }

        /// <summary>
        /// Configures the WebApplication.
        /// </summary>
        private static void ConfigureApplication()
        {
            // Configure the HTTP request pipeline.
            if (App.Environment.IsDevelopment())
            {
                App.UseSwagger();
                App.UseSwaggerUI();
            }
            App.UseHttpsRedirection();
            App.UseRouting();
            App.UseAuthorization();
            App.MapControllers();
            App.UseStaticFiles();
            App.UseFileServer();
            App.UseEndpoints(ep =>
            {
                ep.MapHub<MessageHub>(App.Configuration["messageHub"] ?? "");
            });
        }

        /// <summary>
        /// Creates the <see cref="WebApplication"/>.
        /// </summary>
        /// <param name="args">An array of strings that represents the parameters passed at the execution.</param>
        private static void CreateWebApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();

            App = builder.Build();
        }

        /// <summary>
        /// Loads the <code>urls.json</code> file to the database.
        /// </summary>
        private static void LoadUrlJson()
        {
            var fileName = App.Configuration.GetValue<string>("urls.json");
            if (fileName == null)
                return;

            var logFile = App.Configuration.GetValue<string>("urls.logFile") ?? "urls_json.log";

            var p = Path.GetDirectoryName(Path.GetFullPath(fileName)) ?? "";
            var fn = Path.GetFileNameWithoutExtension(fileName);
            fileName = Path.Combine(p, fn + ".json");
            using var writer = new StreamWriter(Path.Combine(p, logFile), true);
            if (File.Exists(fileName))
            {
                var numInserted = 0;
                try
                {
                    numInserted = ProcessUrlJson(fileName, writer);
                }
                catch (Exception e)
                {
                    LogException(writer, e);
                }
                finally
                {
                    writer.WriteLine($@"    Number of records inserted: {numInserted}");
                    writer.WriteLine($@"--- Concluded at: {DateTime.Now}");
                }

                RenameToOld(fileName);
            }
        }

        /// <summary>
        /// Process the �url.json� file.
        /// </summary>
        /// <param name="fileName">The name of the file to be precessed.</param>
        /// <param name="logger">A <see cref="TextWriter"/> to log the progress to.</param>
        /// <returns>The number of records inserted in the repository.</returns>
        private static int ProcessUrlJson(string fileName, StreamWriter logger)
        {
            var numInserted = 0;
            logger.WriteLine($@"--- Started at: {DateTime.Now}");
            logger.WriteLine($@"    ‘{fileName}’ exists. Attempting to read it.");
            using (var reader = new StreamReader(fileName, true))
            {
                var cvt = JsonSerializer.Deserialize<ShortUrlDTO[]>(reader.ReadToEnd());
                if (cvt != null)
                foreach (var dto in cvt)
                {
                    try
                    {
                        dto.DateCreated = DateTime.Now;
                        ShortUrlRepository.Add(dto);
                        numInserted += 1;
                    }
                    catch (GenericADOException e)
                    {
                        if ((e.InnerException?.GetType() == typeof(PostgresException)) && e.InnerException.Message.StartsWith("23505"))
                            logger.WriteLine($@"    Attempt to insert duplicated values: {JsonSerializer.Serialize(dto)}");
                        else
                            throw;
                    }
                    catch
                    {
                        throw;
                    }
                }
                /*
                 * Fix the sequence in PostgreSQL database
                 */
                if (App.Configuration["NHibernate:dialect"]?.Contains("Postgre", StringComparison.InvariantCultureIgnoreCase) ?? false)
                {
                    var newId = ShortUrlRepository.GetAll().OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault() + 1;
                    using var s = SessionFactory.OpenSession();
                    using var t = s.BeginTransaction();
                    var q = s.CreateSQLQuery($@"SELECT setval('public.""ShortUrls_id_seq""', {newId}, true);");
                    q.ExecuteUpdate();
                }
            }

            return numInserted;
        }

        /// <summary>
        /// Renames the specified file to a file with a �.old� extension.
        /// </summary>
        /// <param name="fileName">The file name to be renamed.</param>
        private static void RenameToOld(string fileName)
        {
            var cnt = 0;
            var path = Path.GetDirectoryName(fileName) ?? "";
            var file = Path.GetFileNameWithoutExtension(fileName);
            var newFileName = Path.Combine(path, file + ".old");
            while (File.Exists(newFileName))
                newFileName = Path.Combine(path, file + $@" ({++cnt}).old");

            File.Move(fileName, newFileName);
        }

        /// <summary>
        /// Writes the specified <see cref="Exception"/> to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write the exception log.</param>
        /// <param name="e">The <see cref="Exception"/> that has occurred.</param>
        private static void LogException(TextWriter writer, Exception e)
        {
            var ident = "";
            writer.WriteLine($@"--- An error has occurred.");
            while (e != null)
            {
                ident += "    ";
                writer.WriteLine(ident + $@"{{{e.GetType().Name}}} : {e.Message}");
                writer.WriteLine();
                writer.WriteLine(ident + $@"Stacktrace:");
                writer.WriteLine(ident + $@"{(e.StackTrace ?? "<null>").Replace("\n", "\n" + ident)}");
                writer.WriteLine(ident + $@"--- End Stacktrace");
                if (e.InnerException != null)
                {
                    writer.WriteLine(ident + $@"--- InnerException:");
                    e = e.InnerException;
                }
                else
                    break;
            }
            writer.WriteLine($@"--- End Error Reporting.");
        }
    }
}