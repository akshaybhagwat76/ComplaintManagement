using System;
using System.IO;
using System.Text;
using System.Web;

namespace ComplaintManagement.Helpers
{
    public class Common
    {

        private Random random = new Random((int)DateTime.Now.Ticks);
        private string RandomString(int Size)
        {
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
        public void RemoveFile(string fileName)
        {
            string filePath = "~/Images/profile_pics/" + fileName;
            if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
            {
                File.Delete(HttpContext.Current.Server.MapPath(filePath));
            }
        }
        public string SaveImageFromBase64(string filename)
        {
            string filePath = "";
            string[] pd = filename.Split(',');
            string NewFileName = RandomString(3) + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + "."+pd[2];
            byte[] imageBytes = Convert.FromBase64String(pd[1]);
            if (!Directory.Exists("~/Images/profile_pics"))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Images/profile_pics"));
            }
            filePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/profile_pics/" + NewFileName);
            File.WriteAllBytes(filePath, imageBytes);
            return NewFileName;
        }
    }
}