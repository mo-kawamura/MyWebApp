using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Models.todoCreate;//追加
using Microsoft.Extensions.Logging;//追加


namespace MyWebApp.Controllers
{
    public class TodoesController : Controller
    {
        private readonly ILogger<TodoesController> _logger;
        private readonly MyWebAppContext _context;


        public TodoesController(ILogger<TodoesController> logger,MyWebAppContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ActionResult> About()
        {
            IQueryable<todoCreateGroup> data =
                from t in _context.Todo
                group t by t.DateStart into dateGroup
                select new todoCreateGroup()
                {
                    EnrollmentDate = dateGroup.Key,
                    PersonCount = dateGroup.Count()
                };
            return View(await data.AsNoTracking().ToListAsync());
        }
        public async Task<IActionResult> DateStartView(string sortOrder, string currentFilter, string FindStr, int? pageNumber, bool check)
        //↑、引数に現在の並べ替え順序パラメーター(1)、および現在のフィルターパラメーター(2)、もともとある検索用(3)ページ番号パラメーター(4)を追加
        {
            //ViewData["～"]は、C#のコードからテンプレートに値の受け渡しする場合に用いられるプロパティ
            //既定の並べ替え順序は昇順("Date"と同じ)
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";//条件? trueの場合 : falseの場合
            ViewData["Checked"] = check ;

            //ソート用
            ViewData["CurrentSort"] = sortOrder;//CurrentFilter という名前の ViewData 要素が現在のフィルター文字列をビューに提供
            //↑ページング中にフィルターの設定を維持するために、ページングのリンクにこの値を含める必要があり、
            //(ないとソートしたときに消えていく)ページが再表示されるときに、この値をテキスト ボックスに復元する必要がる

            if (FindStr != null)
            {
                pageNumber = 1;
            }
            //最初のページ表示、またはユーザーがページングや並べ替えのリンクをクリックしていない場合、すべてのパラメーターは null 
            else
            {
                FindStr = currentFilter;
            }

            ViewData["CurrentFilter"] = FindStr;//フィルター用。上に書かないように（else以降で書き換えが起こってしまう）


            var todoes = from p in _context.Todo select p;
            
            //検索欄に値が入っていれば、その値を検索
            if (!String.IsNullOrEmpty(FindStr))
            //空かnullでない時、Whereで条件を絞り、Includeで外部のデータ(Personクラス)を含めた表示をする
            //Includeないと名前の欄だけ空になる
            {
                todoes = todoes.Where(t => t.Person.Name == FindStr).Include(m => m.Person);
            }
           
            switch (sortOrder)
            {
                case "Date":
                    todoes = todoes.OrderBy(m => m.DateEnd).Include(m => m.Person);
                    break;
                case "date_desc":
                    todoes = todoes.OrderByDescending(m => m.DateEnd).Include(m => m.Person);
                    break;
                default:
                    todoes = todoes.OrderByDescending(m => m.DateEnd).Include(m => m.Person);
                    break;
            }
            int pageSize = 3;
            //return View(await todoes.ToListAsync());//returnは最後にまとめて（条件分岐ごとに書かず、同じ変数を使いまわすこと）書かないと、
            //それ以降処理されなくなるので注意
            return View(await PaginatedList<Todo>.CreateAsync(todoes, pageNumber ?? 1, pageSize));
            //PaginatedList.CreateAsync メソッドが変換してくれる
        }

        // GET: Todoes
        //最初に
        /*
        public async Task<IActionResult> Index()
        {
            var myWebAppContext = _context.Todo.Include(t => t.Person);
            return View(await myWebAppContext.ToListAsync());
        }
        */

        //GET: People 通常と検索用文字列が送信された場合の両方を処理、引数なしのIndexも同時に記述するとエラーになる

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string FindStr, int? pageNumber)
        //↑、引数に現在の並べ替え順序パラメーター(1)、および現在のフィルターパラメーター(2)、もともとある検索用(3)ページ番号パラメーター(4)を追加
        {
           

            //ViewData["～"]は、C#のコードからテンプレートに値の受け渡しする場合に用いられるプロパティ
            //条件? trueの場合 : falseの場合
            //既定の並べ替え順序は昇順("Date"と同じ)
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            
            //ソート用
            ViewData["CurrentSort"] = sortOrder;//CurrentFilter という名前の ViewData 要素が現在のフィルター文字列をビューに提供
            //↑ページング中にフィルターの設定を維持するために、ページングのリンクにこの値を含める必要があり、
            //(ないとソートしたときに消えていく)ページが再表示されるときに、この値をテキスト ボックスに復元する必要がる

            if (FindStr != null)
            {
                pageNumber = 1;
            }
            //最初のページ表示、またはユーザーがページングや並べ替えのリンクをクリックしていない場合、すべてのパラメーターは null 
            else
            {
                FindStr = currentFilter;
            }

            ViewData["CurrentFilter"] = FindStr;//フィルター用。上に書かないように（else以降で書き換えが起こってしまう）


            var todoes = from p in _context.Todo select p;

            //検索欄に値が入っていれば、その値を検索
            if (!String.IsNullOrEmpty(FindStr))
            //空かnullでない時、Whereで条件を絞り、Includeで外部のデータ(Personクラス)を含めた表示をする
            //Includeないと名前の欄だけ空になる
            {
                todoes = todoes.Where(t => t.Person.Name == FindStr).Include(m => m.Person);
            }

            switch (sortOrder)
            {
                case "Date":
                    todoes =todoes.OrderBy(m => m.DateEnd).Include(m => m.Person);
                    break;
                case "date_desc":
                    todoes = todoes.OrderByDescending(m => m.DateEnd).Include(m => m.Person);
                    break;
                default:
                    todoes = todoes.OrderByDescending(m => m.DateEnd).Include(m => m.Person);
                    break;
            }
            int pageSize = 3;
            //return View(await todoes.ToListAsync());//returnは最後にまとめて（条件分岐ごとに書かず、同じ変数を使いまわすこと）書かないと、
            //それ以降処理されなくなるので注意
            return View(await PaginatedList<Todo>.CreateAsync(todoes, pageNumber ?? 1, pageSize));
            //PaginatedList.CreateAsync メソッドが変換してくれる
        }



        // GET: Todoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todo
                .Include(t => t.Person)
                .FirstOrDefaultAsync(m => m.TodoId == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // GET: Todoes/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Name");
            return View();
        }

        // POST: Todoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TodoId,DateStart,DateEnd,Title,content,PersonId")] Todo todo)
        {
            //↓！を付けて送信ができるように
            if (!ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail", "Name");
            //↑Personクラスにある全てのプロパティを記述し、最後に書かれていたもの（todo.PersonId）を消す
            //Editも同じように
            return View(todo);
        }

        // GET: Todoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todo.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId","Name");
            //↑前述(Create)のとおり変更かけるところと思いきや、人物の表示が"Mail"で入ったので消してみたら人物の表示が"Name"に
            //↑多分Personクラスに含まれるIdと表示用の要素(Name)を持ってくるといい？(Name,Mail両方で確認済み)
            return View(todo);
        }

        // POST: Todoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TodoId,DateStart,DateEnd,Title,content,PersonId")] Todo todo)
        {
            if (id != todo.TodoId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)//!
            {
                try
                {
                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todo.TodoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Name");//一応こっちも
            return View(todo);
        }

        // GET: Todoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todo
                .Include(t => t.Person)
                .FirstOrDefaultAsync(m => m.TodoId == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (todo != null)
            {
                _context.Todo.Remove(todo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(int id)
        {
            return _context.Todo.Any(e => e.TodoId == id);
        }
    }
}
