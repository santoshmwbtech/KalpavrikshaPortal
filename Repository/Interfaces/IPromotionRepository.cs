using JBNClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IPromotionRepository
    {
        Task<string> Promotion(PromotionsDTO promotionsDTO, List<Attachment> MailAttachments, string ImageURL, int UserID);
    }
}
