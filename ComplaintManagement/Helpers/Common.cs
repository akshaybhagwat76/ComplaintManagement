using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public void RemoveDoc(string fileName)
        {
            string filePath = "~/Documents/" + fileName;
            if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
            {
                File.Delete(HttpContext.Current.Server.MapPath(filePath));
            }
        }

        public bool GetFilePathExist(string fileName)
        {
            try
            {
                string filePath = "~/Images/profile_pics/" + fileName;

                if (File.Exists(HttpContext.Current.Server.MapPath(filePath)))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int[] StringToIntArray(string values)
        {
            List<int> valuesConvertedInt = new List<int>();
            Array.ForEach(values.Split(",".ToCharArray()), s =>
            {
                int currentInt;
                if (Int32.TryParse(s, out currentInt))
                    valuesConvertedInt.Add(currentInt);
            });
            return valuesConvertedInt.ToArray();
        }
        public string UniqueFileName()
        {
            return DateTime.Now.ToString("ddMMyyyyhhmmss");
        }
        public string SaveImageFromBase64(string filename)
        {
            string filePath = "";
            string[] pd = filename.Split(',');
            string NewFileName = RandomString(3) + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + "." + pd[2];
            byte[] imageBytes = Convert.FromBase64String(pd[1]);
            if (!Directory.Exists("~/Images/profile_pics"))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Images/profile_pics"));
            }
            filePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/profile_pics/" + NewFileName);
            File.WriteAllBytes(filePath, imageBytes);
            return NewFileName;
        }
        public string SaveExcelFromBase64(string filename)
        {

            try
            {
                string filePath = "";
                string[] pd = filename.Split(',');
                string NewFileName = RandomString(3) + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + "." + pd[2];
                byte[] imageBytes = Convert.FromBase64String(pd[1]);
                if (!Directory.Exists("~/Imports/temps"))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Imports/temps"));
                }
                filePath = System.Web.HttpContext.Current.Server.MapPath("~/Imports/temps/" + NewFileName);
                File.WriteAllBytes(filePath, imageBytes);
                return System.Web.HttpContext.Current.Server.MapPath("~/Imports/temps/" + NewFileName);

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string UpdateTokenValue(string jsonString, string TokenName, string newValue)
        {
            JObject jsonObj = (JObject)JsonConvert.DeserializeObject(jsonString);
            jsonObj.Property(TokenName).Value = newValue;
            return JsonConvert.SerializeObject(jsonObj);
        }
        public bool HasSpecialCharacter(string s)
        {
            foreach (var c in s)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return true;
                }
            }
            return false;
        }
    }
}