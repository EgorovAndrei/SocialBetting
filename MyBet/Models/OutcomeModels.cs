using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialBetting.Models
{
    public class MyOutcomeModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public static void WriteFileInDB(List<HttpPostedFileBase> files, int idOutcome)
        {
            BetsDBDataContext context = new BetsDBDataContext();
            foreach (HttpPostedFileBase fileFromPost in files)
            {
                using (var stream = fileFromPost.InputStream)
                {
                    using (var reader = new BinaryReader(fileFromPost.InputStream))
                    {
                        byte[] file = reader.ReadBytes((int)stream.Length);

                        OutcomeFile outcomeFile = new OutcomeFile();
                        outcomeFile.name = fileFromPost.FileName;
                        outcomeFile.file = file;
                        context.OutcomeFiles.InsertOnSubmit(outcomeFile);
                        context.SubmitChanges();

                        BindOutcomeFile bindOF = new BindOutcomeFile();
                        bindOF.idOutcome = idOutcome;
                        bindOF.idOutcomeFile = outcomeFile.id;
                        context.BindOutcomeFiles.InsertOnSubmit(bindOF);
                        context.SubmitChanges();
                    }
                }
            }
        }
    }
}