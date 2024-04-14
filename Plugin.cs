using Terraria;
using TerrariaApi.Server;

using Microsoft.Xna.Framework;

using Terraria.Localization;

using Terraria.IO;
using System.Reflection;


using System.IO.Compression;
using ReLogic.Content.Sources;
using System.Text.RegularExpressions;

namespace WikiLangPackLoader
{
    [ApiVersion(2, 1)]
    public class WikiLangPackLoader : TerrariaPlugin
    {

        public override string Author => "Cai";

        public override string Description => "加载Wiki语言包";

        public override string Name => "中文Wiki语言包加载器";
        public override Version Version => new Version(1, 0, 0, 0);

        public WikiLangPackLoader(Main game)
        : base(game)
        {
            Order = int.MaxValue;
        }
        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this, Init,int.MinValue);
        }

        private void Init(EventArgs args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var services = new GameServiceContainer();
            string resourceName = "WikiLangPackLoader.ResourcePack.zip";
            string filePath = @"tshock/LangResourcePack.zip";
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                resourceStream.CopyTo(fileStream);
            }
            Utils.TryCreatingDirectory(@"tshock/LangResourcePack/");
            ZipFile.ExtractToDirectory(filePath, @"tshock/LangResourcePack/",true);
            File.Delete(filePath);
            var pack = new ResourcePack(services, @"tshock/LangResourcePack/");
            List<IContentSource> list = new List<IContentSource>
            {
                pack.GetContentSource()
            };
            LanguageManager.Instance.UseSources(list);

            Console.ForegroundColor = ConsoleColor.Red; // 设置前景色为红色
            Console.BackgroundColor = ConsoleColor.Yellow; // 设置背景色为黄色
            Console.WriteLine("\n[中文Wiki语言包加载器]语言包已经加载！\n" +
                $"作者：{pack.Author}\n" +
                $"版本：{pack.Version.Major}.{pack.Version.Minor}\n");

            Console.ResetColor(); // 重置为默认颜色


        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GamePostInitialize.Deregister(this, Init);
            }
            base.Dispose(disposing);
        }


    }
}
