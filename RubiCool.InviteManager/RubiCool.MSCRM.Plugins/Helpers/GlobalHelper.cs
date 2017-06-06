using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace RubiCool.MSCRM.Plugins.Helpers
{
    public static class GlobalHelper
    {
        public static PluginStage GetPluginStage(PluginBase.LocalPluginContext localContext)
        {
            if (localContext.PluginExecutionContext.MessageName.Equals("Create"))
            {
                if(localContext.PluginExecutionContext.Stage == 10)
                    return PluginStage.PreValidateCreate;
                if(localContext.PluginExecutionContext.Stage == 20)
                    return PluginStage.PreOperationCreate;
                if(localContext.PluginExecutionContext.Stage == 40)
                    return PluginStage.PostOperationCreate;
            }
            if (localContext.PluginExecutionContext.MessageName.Equals("Update"))
            {
                if (localContext.PluginExecutionContext.Stage == 10)
                    return PluginStage.PreValidateUpdate;
                if (localContext.PluginExecutionContext.Stage == 20)
                    return PluginStage.PreOperationUpdate;
                if (localContext.PluginExecutionContext.Stage == 40)
                    return PluginStage.PostOperationUpdate;
            }
            return PluginStage.Undefined;
        }

        public static T RetrieveValue<T>(Entity e, string attributeName)
        {
            if (typeof(T) == typeof(Money))
            {
                return e.Contains(attributeName) ? e.GetAttributeValue<T>(attributeName) : (T)(object)new Money();
            }
            if (typeof(T) == typeof(EntityReference))
            {
                return e.Contains(attributeName) ? e.GetAttributeValue<T>(attributeName) : (T)(object)new EntityReference();
            }

            if (typeof(T) == typeof(DateTime))
            {
                return e.Contains(attributeName) ? (T)(object)(e.GetAttributeValue<DateTime>(attributeName).ToLocalTime()) : default(T);
            }
            if (typeof(T) == typeof(DateTime?))
            {
                return e.Contains(attributeName) ? (T)(object)(e.GetAttributeValue<DateTime?>(attributeName).Value.ToLocalTime()) : default(T);
            }
            return e.Contains(attributeName) ? e.GetAttributeValue<T>(attributeName) : default(T);
        }

        public static T RetrieveAliasedValue<T>(Entity e, string attributename)
        {
            if (e.Contains(attributename))
            {
                return (T)e.GetAttributeValue<AliasedValue>(attributename).Value;
            }
            if (typeof(T) == typeof(EntityReference))
            {
                return (T)(object)new EntityReference();
            }
            if (typeof(T) == typeof(Money))
            {
                return (T)(object)new Money();
            }
            return default(T);
        }

        public static T RetrieveAliasedValue<T>(Entity e, string attributename, bool throwErrorWhenNull)
        {
            if (e.Contains(attributename))
            {
                return (T)e.GetAttributeValue<AliasedValue>(attributename).Value;
            }
            throw new InvalidPluginExecutionException(
                $"Attribuut {attributename} (Entiteit {e.LogicalName}) bevat geen waarde");
        }

        public static T RetrieveValueWithImage<T>(Entity e, Entity image, string attributeName)
        {
            return e.Contains(attributeName) ? e.GetAttributeValue<T>(attributeName) :
                image != null ? image.GetAttributeValue<T>(attributeName) : default(T);
        }

        public static Entity RetrieveEntity(IPluginExecutionContext context)
        {
            return (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity) ? (Entity)context.InputParameters["Target"] : null;
        }

        public static Entity RetrievePreImageEntity(IPluginExecutionContext context, string preImageName)
        {
            return (context.PreEntityImages.Contains(preImageName) && context.PreEntityImages[preImageName] is Entity) ? (Entity)context.PreEntityImages[preImageName] : null;
        }

        public static Entity RetrievePostImageEntity(IPluginExecutionContext context, string postImageName)
        {
            return (context.PostEntityImages.Contains(postImageName) && context.PostEntityImages[postImageName] is Entity) ? (Entity)context.PostEntityImages[postImageName] : null;
        }

        public static string RetrieveOptionSetLabel(IOrganizationService service, string entityName, string attributeName, int optionSetValue)
        {
            RetrieveAttributeRequest req = new RetrieveAttributeRequest();
            req.EntityLogicalName = entityName;
            req.LogicalName = attributeName;
            req.RetrieveAsIfPublished = true;

            PicklistAttributeMetadata metaData = (PicklistAttributeMetadata)((RetrieveAttributeResponse)service.Execute(req)).AttributeMetadata;
            return metaData.OptionSet.Options.First<OptionMetadata>(o => o.Value == optionSetValue).Label.UserLocalizedLabel.Label;
        }

        public static void SetEntityValue(Entity e, string attributeName, object value)
        {
            if (value == null)
                return;

            if (value.GetType() == typeof(DateTime?))
            {
                DateTime? dt = (DateTime?)value;
                if (dt != null)
                {
                    if (dt.Value == DateTime.MinValue)
                    {
                        e.Attributes.Add(attributeName, null);
                    }
                    else
                    {
                        e.Attributes.Add(attributeName, dt.Value);
                    }
                }
                return;
            }
            if (value.GetType() == typeof(DateTime))
            {
                DateTime dt = (DateTime)value;
                if (dt != DateTime.MinValue)
                {
                    e.Attributes.Add(attributeName, dt);
                }
                else
                {
                    e.Attributes.Add(attributeName, null);
                }
                return;
            }
            e.Attributes.Add(attributeName, value);
        }
        public static void SetEntityValue(Entity e, string attributeName, decimal value, bool isMoney)
        {
            SetEntityValue(e, attributeName, new Money(value));
        }

        public static DateTime RetrieveMinimalCrmDatetime()
        {
            return new DateTime(1900, 1, 1);
        }

        /// <summary>
        /// JSON Serialization
        /// </summary>
        public static string JsonSerializer<T>(T t)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, t);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }

    public enum PluginStage
    {
        PreValidateCreate,
        PreValidateUpdate,
        PreOperationCreate,
        PreOperationUpdate,
        PostOperationCreate,
        PostOperationUpdate,
        Undefined
    }
}
