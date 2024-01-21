using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XSystem.Security.Cryptography;

namespace IakademiWebProject.Models
{
	public class Cls_User
	{
		iakademi38Context _context = new ();

		public async Task<User> LoginControl(User user)
		{
			string mD5Sifre = MD5Sifrele(user.Password);

			User? usr = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == mD5Sifre 
																&& u.IsAdmin == true && u.Active == true);

			return usr;
		}

		public static string MD5Sifrele(string password)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] btr = Encoding.UTF8.GetBytes(password);
			btr = md5.ComputeHash(btr);

			StringBuilder sb = new();
			foreach (byte b in btr) 
			{ 
				sb.Append(b.ToString("x2").ToLower());
			}

			return sb.ToString();
		
		}

		public static User? SelectMemberInfo(string email)
		{
			try
			{
				using (iakademi38Context _context = new()) 
				{  
					User? user = _context.Users.FirstOrDefault(u=>u.Email == email);
					return user;
				}

			}
			catch
			{
				return null;
			}
		}

        public static string MemberControl(User user)
        {
            using (iakademi38Context context = new iakademi38Context())
            {
                string answer = "";

                try
                {
                    string md5Sifre = MD5Sifrele(user.Password);
                    User? usr = context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == md5Sifre);

                    if (usr == null)
                    {
                        //kullanıcı yanlıs sifre veya emal girdi
                        answer = "error";
                    }
                    else
                    {
                        //kullanıcı veritabanında kayıtlı.
                        if (usr.IsAdmin == true)
                        {
                            //admin yetkisi olan personel giriş yapıyor
                            answer = "admin";
                        }
                        else
                        {
                            answer = usr.Email;
                        }
                    }
                }
                catch (Exception)
                {
                    return "HATA";
                }
                return answer;
            }
        }

		public static bool LoginEmailControl(string email) 
		{
			try
			{
				bool result = true;
				using (iakademi38Context _context = new())
				{
					result = _context.Users.Any(e => e.Email == email);
                }
				return result;
            }
			catch 
			{
				return true;
			}
		
		}

		public static bool AddUser(User user)
		{
			try
			{
				user.Active = true;
				user.IsAdmin = false;
				user.Password = MD5Sifrele(user.Password);
                using (iakademi38Context _context = new())
                {
				_context.Users.Add(user);
                 _context.SaveChanges();
                }
                return true;
            }
			catch 
			{
				return false;
			}
		}

        public static void Send_Sms(string OrderGroupGUID)
        {
            using (iakademi38Context context = new iakademi38Context())
            {
                string ss = "";
                ss += "<?xml version='1.0' encoding='UTF-8' >";
                ss += "<mainbody>";
                ss += "<header>";
                ss += "<company dil='TR'>iakademi(üye oldugunuzda size verilen şirket ismi)</company>";
                ss += "<usercode>0850 size verilen user kod burada yazılacak</usercode>";
                ss += "<password>NetGsm123. size verilen şifre burada yazılacak</password>";
                ss += "<startdate></startdate>";
                ss += "<stopdate></stopdate>";
                ss += "<type>n:n</type>";
                ss += "<msgheader>başlık buraya</msgeader>";
                ss += "</header>";
                ss += "<body>";

                Order order = context.Orders.FirstOrDefault(o => o.OrderGroupGUID == OrderGroupGUID);
                User user = context.Users.FirstOrDefault(u => u.UserID == order.UserID);
                //Sayın Sedat tefçi, 05 04 2023 tarihinde 5042023194420 nolu siparişiniz alınmıştır.
                string content = "Sayın " + user.NameSurname + "," + DateTime.Now + " tarihinde " + OrderGroupGUID + " nolu siparişiniz alınmıştır.";

                ss += "<mp><msg><![CDATA[" + content + "]]></msg><no>90" + user.Telephone + "</no></mp>";
                ss += "</body>";
                ss += "</mainbody>";

                string answer = XMLPOST("https://api.netgsm.com/tr/xmlbulkhttppost.asp", ss);
                if (answer != "-1")
                {
                    //sms gitti
                }
                else
                {
                    //sms gitmedi
                }
            }
        }

        public static string XMLPOST(string url, string xmlData)
        {
            try
            {
                WebClient wUpload = new WebClient();
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest; //Convert = CASTING
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                Byte[] bPostArray = Encoding.UTF8.GetBytes(xmlData);
                Byte[] bResonse = wUpload.UploadData(url, "POST", bPostArray);

                Char[] sReturnsChars = Encoding.UTF8.GetChars(bResonse);

                string sWebPage = new string(sReturnsChars);
                return sWebPage;
            }
            catch (Exception)
            {
                return "-1";
            }
        }

        public static void Send_Email(string OrderGroupGUID)
        {
            using (iakademi38Context context = new iakademi38Context())
            {
                Order order = context.Orders.FirstOrDefault(o => o.OrderGroupGUID == OrderGroupGUID);
                User user = context.Users.FirstOrDefault(u => u.UserID == order.UserID);

                string mail = "gonderen email buraya info@iakademi.com";
                string _mail = user.Email;
                string subject = "";
                string content = "";

                content = "Sayın " + user.NameSurname + "," + DateTime.Now + " tarihinde " + OrderGroupGUID + " nolu siparişiniz alınmıştır.";

                subject = "Sayın " + user.NameSurname + " siparişiniz alınmıştır.";

                string host = "smtp.iakademi.com";
                int port = 587;
                string login = "mailserver a baglanılan login buraya";
                string password = "mailserver a baglanılan şifre buraya";

                MailMessage e_posta = new MailMessage();
                e_posta.From = new MailAddress(mail, "iakademi bilgi"); //gönderen
                e_posta.To.Add(_mail); //alıcı
                e_posta.Subject = subject;
                e_posta.IsBodyHtml = true;
                e_posta.Body = content;

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new NetworkCredential(login, password);
                smtp.Port = port;
                smtp.Host = host;

                try
                {
                    smtp.Send(e_posta);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

    }
}
