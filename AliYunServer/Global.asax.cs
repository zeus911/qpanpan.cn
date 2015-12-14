using Models;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;

namespace AliYunServer
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // EF暖机操作（加载映射到内存中）
            using (qds173643455_db dbContext = new qds173643455_db())
            using (ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext)
            {
                StorageMappingItemCollection mapppingCollection =
                    (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                mapppingCollection.GenerateViews(new List<EdmSchemaError>());
            }

            log4net.Config.XmlConfigurator.Configure();
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = "" }
              );
        }

        protected void Application_BeginRequest()
        {
            if (Request.Url.AbsolutePath == "/")
            {
                Context.RewritePath("~/index.shtml");
            }
        }

        protected void Application_Error()
        {
            ILog log = LogManager.GetLogger(typeof(MvcApplication));
            log.Error("系统发生未处理异常", Context.Error);
        }
    }
}