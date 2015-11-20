using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace SocialBetting.Models
{
    public class MyBetModel
    {
        [ScaffoldColumn(false)]
        public int id { get; set; }
    
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }

        public List<MyOutcomeModel> Outcomes = new List<MyOutcomeModel>();

        public static void WriteFileInDB(List<HttpPostedFileBase> files,int idBet)
        {
            BetsDBDataContext context = new BetsDBDataContext();
            foreach (HttpPostedFileBase fileFromPost in files)
            {
                using (var stream = fileFromPost.InputStream)
                {
                    using (var reader = new BinaryReader(fileFromPost.InputStream))
                    {
                        byte[] file = reader.ReadBytes((int)stream.Length);

                        BetFile betfile = new BetFile();
                        betfile.name = fileFromPost.FileName;
                        betfile.file = file;
                        context.BetFiles.InsertOnSubmit(betfile);
                        context.SubmitChanges();

                        BindBetFile bindBF = new BindBetFile();
                        bindBF.idBet = idBet;
                        bindBF.idBetFile = betfile.id;
                        context.BindBetFiles.InsertOnSubmit(bindBF);
                        context.SubmitChanges();
                    }
                }
            }

        }

        public static void ReadFileFromDB()
        {
            BetsDBDataContext context = new BetsDBDataContext();
            var file = context.BetFiles.Where(f => f.name == "1").FirstOrDefault();
            var t = file.file;
        }
    }

    

}