using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyWebApp
{

    //参照サイト↓
    //https://learn.microsoft.com/ja-jp/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-8.0マイクロソフトの公式チュートリアル
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;//番号
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);//含まれるページのリストの量

            AddRange(items);
        }

        //↓HasPreviousPage および HasNextPage を使用して、 [前のページ] および [次のページ] ページング ボタンを有効または無効に
        public bool HasPreviousPage => PageIndex > 1;//1よりインデックスが大きいとき、前にいくページを有効に

        public bool HasNextPage => PageIndex < TotalPages;//トータルよりインデックスが小さいとき、次にいくページを有効に

        //コンストラクターは非同期コード(asyncついてるのでたぶんそう)を実行できないので、
        //代わりに CreateAsync メソッドを使用して PaginatedList<T>オブジェクトを作成

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        //CreateAsync メソッドは、ページ サイズとページ番号を受け取り、適切な Skip および Take ステートメントを IQueryable に適用。
        //IQueryable で ToListAsync が呼び出されると、要求されたページのみを含むリストを返す（チュートリアルより）
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            //IQueryable で ToListAsync ↑が呼び出されると、要求されたページのみを含むリストを返す↓
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}