using System;
using System.Collections.Generic;
using System.IO;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;
using static System.Net.Mime.MediaTypeNames;


public class SmtpMessageChunk
{

    public static void Main(string[] args)
    {
        SendMessageSmtp();
    }

    public static void SendMessageSmtp()
    {
        Console.WriteLine("Mail listesinin içinde bulunduğu dosya yolu (C:\\Users\\Gorke\\Desktop\\okunacakmetin.txt): ");
        string okunacakMetin = Console.ReadLine();
        Console.WriteLine("Eklenecek PDF'in bulunduğu dosya yolu  (C:\\Users\\Gorke\\Desktop\\price-list.pdf): ");
        string eklenecekDosya = Console.ReadLine();

        List<string> satirlarList = new List<string>();
        using (StreamReader sr = new StreamReader(okunacakMetin))
        {
            int i = 0;
            string satir;
            while ((satir = sr.ReadLine()) != null)
            {
                i++;
                satir = satir.Trim();
                satir = satir.Replace('&', '@');
                satirlarList.Add(satir);
                Console.WriteLine(i +" - " + satir.Split("@")[0] + " -> " + satir.ToLower() + " -> Gönderim Başarılı ! ");
                // Compose a message
                MimeMessage mail = new MimeMessage();

                mail.From.Add(new MailboxAddress("Görkem", "gorkem@gorkemnet.com"));

                mail.To.Add(new MailboxAddress(satir.Split("@")[0], satir.ToLower()));

                #region Mail Subject
                mail.Subject = "Price Offer";
                #endregion
                #region Message Body

                var builder = new BodyBuilder();
                builder.TextBody = @"Good Day Dear Sir,

    I am directing the list of children's playground materials that we produce
in the attached file.

    We would like to meet and work with you in our commercial life that we
started in 1992.

    The material quality and prices of our products that we produce in
Istanbul-Turkey will draw your attention.

    Contact us for technical details and requests about the products you are
interested in.

    Görkem SANDIKCI.

    Our price offers are valid for 15 (fifteen) days.
";
                builder.HtmlBody = string.Format(@"<p>Good Day Dear Sir,<br><br>
I am directing the list of children's playground materials that we produce
in the attached file.<br>
We would like to meet and work with you in our commercial life that we
started in 1992.<br>
 The material quality and prices of our products that we produce in
Istanbul-Turkey will draw your attention.<br>
    Contact us for technical details and requests about the products you are
interested in.<br>
Görkem SANDIKCI.<br>

    Our price offers are valid for 15 (fifteen) days.<br>
</p>");
                #endregion
                #region Attachments
                builder.Attachments.Add(eklenecekDosya);
            #endregion
                #region Message Body Set
                // Now we just need to set the message body and we're done
                mail.Body = builder.ToMessageBody();
                #endregion
                #region Send Method
                // Send it!
                using (var client = new SmtpClient())
                {
                    // XXX - Should this be a little different?
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.eu.mailgun.org", 587, false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate("postmaster@domain.com", "49ace8614ef41e9fe810bce685d32874-d6b089cc-e2e3d8ec");

                    client.Send(mail);
                    client.Disconnect(true);
                }
                #endregion
            }
            Console.WriteLine("Mailler Başarıyla Gönderildi! \n mailList.txt dosyasını sıfırlamayı unutmayınız!");
            Console.ReadLine();
        }
      
    }

}
