using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using RubiCool.MSCRM.Plugins.Helpers;

namespace RubiCool.MSCRM.Plugins
{
    public class Account : PluginBase
    {
        public Account(string unsecure, string secure)
            : base(typeof(Contact))
        {
            SettingHelper.InitSecureConfig(secure);
        }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The <see cref="PluginBase.LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("localContext");
            }

            var context = localContext.PluginExecutionContext;

            // Obtain the target entity from the input parameters
            var entity = GlobalHelper.RetrieveEntity(context);

            // Verify that the target entity represents an entity type you are expecting
            if (entity.LogicalName != "account")
                return;

            var preImageEntity = GlobalHelper.RetrievePreImageEntity(context, "PreImage");
            var postImageEntity = GlobalHelper.RetrievePostImageEntity(context, "PostImage");

            var pluginStage = GlobalHelper.GetPluginStage(localContext);
            localContext.Trace($"pluginStage: '{pluginStage}'");
            switch (pluginStage)
            {
                case PluginStage.PostOperationCreate:
                    AccountPluginContextHelper.CreateGroup(localContext.OrganizationService,
                        localContext.TracingService, entity, postImageEntity);
                    break;
                case PluginStage.PreOperationUpdate:
                    break;
                case PluginStage.PostOperationUpdate:
                    break;
            }
        }

    }
}
