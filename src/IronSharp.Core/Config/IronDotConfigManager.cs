using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using Common.Logging;
using Newtonsoft.Json;

namespace IronSharp.Core
{
    /// <summary>
    /// http://dev.iron.io/cache/reference/configuration/
    /// </summary>
    public static class IronDotConfigManager
    {
        public static string GetEnvironmentKey(IronProduct product, string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            string productName = GetProductName(product);

            return string.Format("{0}_{1}", productName, key).ToUpper();
        }

        public static T GetEnvironmentValue<T>(IronProduct product, string key, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            return GetEnvironmentValue(product, key, default(T), target);
        }

        public static T GetEnvironmentValue<T>(IronProduct product, string key, T defaultValue, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            return Environment.GetEnvironmentVariable(GetEnvironmentKey(product, key), target).As(defaultValue);
        }

        public static IronClientConfig Load(IronProduct product = IronProduct.AllProducts, IronClientConfig overrideConfig = null, string env = null)
        {
            string homeIronDotJson = Path.Combine(GetHomeDirectory(), ".iron.json");
            string appIronDotJson = Path.Combine(GetAppDirectory(), "iron.json");

            IronClientConfig home = ParseJsonFile(product, homeIronDotJson, env);
            IronClientConfig app = ParseJsonFile(product, appIronDotJson, env);

            ApplyOverrides(home, app);
            ApplyOverrides(home, overrideConfig);

            return home;
        }

        public static IronClientConfig LoadFromEnvironment(IronProduct product, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            var baseConfig = new IronClientConfig
            {
                ProjectId = GetEnvironmentValue<string>(IronProduct.AllProducts, "projectid", target),
                Token = GetEnvironmentValue<string>(IronProduct.AllProducts, "token", target)
            };

            var productConfig = new IronClientConfig
            {
                ProjectId = GetEnvironmentValue<string>(product, "projectid", target),
                Token = GetEnvironmentValue<string>(product, "token", target),
                Host = GetEnvironmentValue<string>(product, "host", target)
            };

            ApplyOverrides(baseConfig, productConfig);

            return baseConfig;
        }

        public static IronClientConfig ParseJsonFile(IronProduct product, string filePath, string env = null)
        {
            JsonDotConfigModel config = LoadJson(filePath, env);

            var settings = new IronClientConfig
            {
                ProjectId = config.ProjectId,
                Token = config.Token,
                Host = config.Host
            };

            ApplyOverrides(settings, GetProductOverride(product, config));

            return settings;
        }

        public static void SetEnvironmentValue(IronProduct product, string key, string value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            Environment.SetEnvironmentVariable(GetEnvironmentKey(product, key), value, target);
        }

        public static void DeleteEnvironmentValue(IronProduct product, string key, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            SetEnvironmentValue(product, key, null, target);
        }

        private static void ApplyOverrides(IronClientConfig targetConfig, IronClientConfig overrideConfig)
        {
            if (overrideConfig == null)
            {
                return;
            }

            targetConfig.ProjectId = string.IsNullOrEmpty(overrideConfig.ProjectId) ? targetConfig.ProjectId : overrideConfig.ProjectId;
            targetConfig.Token = string.IsNullOrEmpty(overrideConfig.Token) ? targetConfig.Token : overrideConfig.Token;
            targetConfig.Host = string.IsNullOrEmpty(overrideConfig.Host) ? targetConfig.Host : overrideConfig.Host;
        }

        private static string _appDirectory;

        public static void SetAppDirectory(string path)
        {
            _appDirectory = path;
        }

        private static string GetAppDirectory()
        {
            if (string.IsNullOrEmpty(_appDirectory))
            {
                _appDirectory = HostingEnvironment.IsHosted ? 
                    HostingEnvironment.MapPath("~/") : 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            }
            return _appDirectory;
        }

        private static string GetHomeDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private static string GetProductName(IronProduct product)
        {
            string productName;

            switch (product)
            {
                case IronProduct.AllProducts:
                    productName = "iron";
                    break;
                case IronProduct.IronMQ:
                    productName = "iron_mq";
                    break;
                case IronProduct.IronWorker:
                    productName = "iron_worker";
                    break;
                case IronProduct.IronCache:
                    productName = "iron_cache";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("product");
            }
            return productName;
        }

        private static IronClientConfig GetProductOverride(IronProduct product, JsonDotConfigModel config)
        {
            IronClientConfig productOverride = null;

            switch (product)
            {
                case IronProduct.IronCache:
                    productOverride = config.IronCache;
                    break;
                case IronProduct.IronMQ:
                    productOverride = config.IronMQ;
                    break;
                case IronProduct.IronWorker:
                    productOverride = config.IronWoker;
                    break;
            }
            return productOverride;
        }

        private static JsonDotConfigModel LoadJson(string filePath, string env = null)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);

                    if (!string.IsNullOrEmpty(env))
                    {
                        var envType = JsonConvert.DeserializeObject<Dictionary<string, JsonDotConfigModel>>(content, new JsonSerializerSettings
                        {
                            Error = (sender, args) =>
                            {
                                args.ErrorContext.Handled = true;
                            }
                        });
                        JsonDotConfigModel envSpecific;
                        if (envType.TryGetValue(env, out envSpecific))
                        {
                            return envSpecific;
                        }
                    }
                    return JsonConvert.DeserializeObject<JsonDotConfigModel>(content);
                }
            }
            catch (IOException ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
            }
            return new JsonDotConfigModel();
        }

        private class JsonDotConfigModel : IronClientConfig
        {
            [JsonProperty("iron_cache")]
            public IronClientConfig IronCache { get; set; }

            [JsonProperty("iron_mq")]
            public IronClientConfig IronMQ { get; set; }

            [JsonProperty("iron_worker")]
            public IronClientConfig IronWoker { get; set; }
        }
    }
}