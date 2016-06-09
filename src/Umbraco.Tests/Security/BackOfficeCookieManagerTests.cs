using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Microsoft.Owin;
using Moq;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Profiling;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using Umbraco.Web.Security.Identity;

namespace Umbraco.Tests.Security
{
    [TestFixture]
    public class BackOfficeCookieManagerTests
    {
        [Test]
        public void ShouldAuthenticateRequest_When_Not_Configured()
        {
            //should force app ctx to show not-configured
            ConfigurationManager.AppSettings.Set("umbracoConfigurationStatus", "");

            var databaseFactory = TestObjects.GetIDatabaseFactoryMock(false);
            var dbCtx = new Mock<DatabaseContext>(databaseFactory, Mock.Of<ILogger>());

            var appCtx = new ApplicationContext(
                dbCtx.Object,
                TestObjects.GetServiceContextMock(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>()));

            var umbCtx = UmbracoContext.CreateContext(
                Mock.Of<HttpContextBase>(), appCtx,
                Mock.Of<IFacadeService>(),
                new WebSecurity(Mock.Of<HttpContextBase>(), appCtx),
                Mock.Of<IUmbracoSettingsSection>(), new List<IUrlProvider>());

            var mgr = new BackOfficeCookieManager(Mock.Of<IUmbracoContextAccessor>(accessor => accessor.UmbracoContext == umbCtx));

            var result = mgr.ShouldAuthenticateRequest(Mock.Of<IOwinContext>(), new Uri("http://localhost/umbraco"));

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldAuthenticateRequest_When_Configured()
        {
            var databaseFactory = TestObjects.GetIDatabaseFactoryMock();
            var dbCtx = new Mock<DatabaseContext>(databaseFactory, Mock.Of<ILogger>());

            var appCtx = new ApplicationContext(
                dbCtx.Object,
                TestObjects.GetServiceContextMock(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>()));

            var umbCtx = UmbracoContext.CreateContext(
                Mock.Of<HttpContextBase>(), appCtx,
                Mock.Of<IFacadeService>(),
                new WebSecurity(Mock.Of<HttpContextBase>(), appCtx),
                Mock.Of<IUmbracoSettingsSection>(), new List<IUrlProvider>());

            var mgr = new BackOfficeCookieManager(Mock.Of<IUmbracoContextAccessor>(accessor => accessor.UmbracoContext == umbCtx));

            var request = new Mock<OwinRequest>();
            request.Setup(owinRequest => owinRequest.Uri).Returns(new Uri("http://localhost/umbraco"));

            var result = mgr.ShouldAuthenticateRequest(
                Mock.Of<IOwinContext>(context => context.Request == request.Object),
                new Uri("http://localhost/umbraco"));

            Assert.IsTrue(result);
        }

        //TODO : Write remaining tests for `ShouldAuthenticateRequest`
    }
}