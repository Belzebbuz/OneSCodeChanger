namespace OneSCodeChanger
{
    public class OneSModule
    {
        public string BasePath { get; set; }
        public string ModuleName { get; set; }
    }

    public class OneSUploadModule : OneSModule
    {
        public string ModuleText { get; set; }
        public bool UpdateDB { get; set; }
    }
}
