
using NHibernate.Mapping.Attributes;
using System.Reflection;
using URLShortener.Model;
using URLShortener.Repositories;

namespace URLShortener
{
    public class Program
    {
        /// <summary>
        /// Gets or sets the <see cref="NHibernate.ISessionFactory"/>.
        /// </summary>
        public static NHibernate.ISessionFactory SessionFactory { get; private set; }

        /// <summary>
        /// Gets or sets the current <see cref="WebApplication"/>.
        /// </summary>
        public static WebApplication App { get; private set; }

        /// <summary>
        /// Gets the <see cref="ShortUrlRepository"/> for the application.
        /// </summary>
        public static ShortUrlRepository ShortUrlRepository { get; private set; } = new ShortUrlRepository();

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
        /// Configures the NHibernate, based on the “appsettings.json” file.
        /// </summary>
        private static void ConfigureNHibernate()
        {
            // Configure NHibernate
            var cfg = new NHibernate.Cfg.Configuration();
            var nHibernateSection = App.Configuration.GetSection("Hibernate");
            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, nHibernateSection.GetValue<string>("dialect"));
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, nHibernateSection.GetValue<string>("driver"));
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
            App.UseAuthorization();
            App.MapControllers();
            App.UseStaticFiles();
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

            App = builder.Build();
        }

        /// <summary>
        /// Loads the <code>urls.json</code> file to the database.
        /// </summary>
        private static void LoadUrlJson()
        {
            return;
        }
    }
}