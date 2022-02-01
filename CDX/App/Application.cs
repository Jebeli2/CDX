namespace CDX.App
{
    using CDX.GUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Application
    {
        private static CDXApplication? app;
        public static void Run(Window window)
        {
            Run(new ApplicationContext(new Configuration(), window));
        }
        public static void Run(params Window[] windows)
        {
            Run(new ApplicationContext(new Configuration(), windows));
        }
        public static void Run(Configuration configuration, Window window)
        {
            Run(new ApplicationContext(configuration, window));
        }
        public static void Run(Configuration configuration, params Window[] windows)
        {
            Run(new ApplicationContext(configuration, windows));
        }
        public static void Run(ApplicationContext context)
        {
            app = CreateApp(context.Configuration.CDXName);
            app?.Run(context);
        }

        //public static void AddWindow(Window window)
        //{
        //    if (app != null)
        //    {
        //        app.AddWindow(window);
        //        window.GUI = CreateGUISystem(configuration.GUIName);
        //    }
        //}

        private static CDXApplication? CreateApp(string appName)
        {
            string[] parts = appName.Split('.');
            if (parts.Length == 2)
            {
                return CreateApp(parts[0], parts[1]);
            }
            return null;
        }
        private static CDXApplication? CreateApp(string appAssembly, string appClass)
        {
            CDXApplication? app = Activator.CreateInstance(appAssembly, appAssembly + "." + appClass)?.Unwrap() as CDXApplication;
            return app;
        }
        internal static IGUISystem CreateGUISystem(string guiName)
        {
            string[] parts = guiName.Split('.');
            if (parts.Length == 2)
            {
                return CreateGUISystem(parts[0], parts[1]);
            }
            return new NoGUI();
        }
        private static IGUISystem CreateGUISystem(string guiAssembly, string guiClass)
        {
            IGUISystem? gui = Activator.CreateInstance(guiAssembly, guiAssembly + "." + guiClass)?.Unwrap() as IGUISystem;
            if (gui != null) { return gui; }
            return new NoGUI();

        }
    }
}
