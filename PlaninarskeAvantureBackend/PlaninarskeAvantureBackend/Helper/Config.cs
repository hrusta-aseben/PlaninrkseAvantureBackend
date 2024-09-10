namespace FIT_Api_Example.Helper
{
    public class Config
    {
        public static string AplikacijURL = "https://api.p2329.app.fit.ba/";

        public static string Slike => "images/";
        public static string SlikeURL => AplikacijURL + Slike;
        public static string SlikeFolder => "wwwroot/" + Slike;
    }
}
