using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Todo
    {
        public int TodoId { get; set; }
        //開始
        [Display(Name = "開始時刻")]
        [Required(ErrorMessage = "開始日時を入力して下さい")]
        public DateTime DateStart { get; set; }
        //終了
        [Display(Name = "終了時刻")]
        [Required(ErrorMessage ="終了日時を入力して下さい")]
        public DateTime DateEnd { get; set; }
        //リストのタイトル
        [Display(Name = "タイトル")]
        [Required(ErrorMessage = "タイトルを入力して下さい")]
        public string Title { get; set; }
        //リストの内容（空でもオッケーにする）
        [Display(Name = "詳細")]
        [Required(ErrorMessage = "詳細を入力して下さい")]
        public string content { get; set; }

        //外部を使用
        public int PersonId { get; set; }//プライマリーキー（外部キー）を保管
        public Person Person { get; set; }//Personインスタンスを保管
    }
}
