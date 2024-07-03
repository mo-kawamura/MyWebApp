using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class PeopleController : Controller
    {
        private readonly MyWebAppContext _context;

        public PeopleController(MyWebAppContext context)
        {
            _context = context;
        }

        // GET: People
        /*
        public async Task<IActionResult> Index()
        {
            return View(await _context.Person.ToListAsync());

        }
        */
        //GET: People 検索用メソッドありの場合
        public async Task<IActionResult> Index(string strSerch)
        {
            if (_context.Person == null)
            {
                return Problem("Entity set 'WebApplication1Context.Employee'  is null.");
            }

            //クエリを使用して、DBのデータを全件取得
            var people = from E in _context.Person
                         select E;

            //検索欄に値が入っていれば、その値を検索
            if (!String.IsNullOrEmpty(strSerch))
            {
                people = people.Where(e => e.Name.Contains(strSerch));
            }

            return View(await people.ToListAsync());
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create　シンプルにリターンしているだけ
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //POST送信を行うクリエイト
        [HttpPost]//post送信を受付
        [ValidateAntiForgeryToken]//外部からのフォーム送信を拒否し、このアクションのみ受付ける。
        public async Task<IActionResult> Create([Bind("PersonId,Name,Mail")] Person person)
            //送信されたフォームをPersonインスタンスとして引数で受け取る
        {
            if (!ModelState.IsValid)//検証処理
            {
                _context.Add(person);//_contextに追加
                await _context.SaveChangesAsync();//保存
                return RedirectToAction(nameof(Index));//リダイレクト（インスタンスを返す）
            }
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,Name,Mail")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
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
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person != null)
            {
                _context.Person.Remove(person);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.PersonId == id);
        }
    }
}
