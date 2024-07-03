using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Person
    {
        public int PersonId { get; set; }

        [Display(Name = "名前")]
        [Required(ErrorMessage = "必須項目です。")]
        public string Name { get; set; }

        [Display(Name = "メールアドレス")]
        [Required(ErrorMessage = "メールアドレスが必要です。")]
        public string? Mail { get; set; }

        //追加
        public ICollection<Person> Persons { get; set;}//<>内は使用するPersonクラス
    }
}
