using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemoCRUD.AcessoDados;
using DemoCRUD.Models;

namespace DemoCRUD.Controllers
{
    public class LivrosController : Controller
    {
        private LivroContexto db = new LivroContexto();

        // GET: Livros
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Listar(string searchPhrase, int current = 1, int rowCount = 5)
        {
            string chave = Request.Form.AllKeys.Where(k => k.StartsWith("sort")).First();

            string ordenacao = Request[chave];
            string campo = chave.Replace("sort[", String.Empty).Replace("]", String.Empty);

            var livros = db.Livros.Include(l => l.Genero);

            int total = livros.Count();

            if (!String.IsNullOrWhiteSpace(searchPhrase))
            {
                int ano = 0;
                int.TryParse(searchPhrase, out ano);

                decimal valor = 0.0m;
                decimal.TryParse(searchPhrase, out valor);

                livros = livros.Where("Titulo.Contains(@0) OR Autor.Contains(@0) OR AnoEdicao == @1 OR Valor= @2", searchPhrase, ano, valor);
            }

            string campoOrdenacao = String.Format("{0} {1}", campo, ordenacao);

            var livrosPaginados = livros.OrderBy(campoOrdenacao).Skip((current - 1) * rowCount).Take(rowCount);

            return Json(new {
                                rows = livrosPaginados.ToList(),
                                current = current,
                                rowCount = rowCount,
                                total = total
                             }
                             , JsonRequestBehavior.AllowGet); ;
        }



        // GET: Livros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livro livro = db.Livros.Find(id);
            if (livro == null)
            {
                return HttpNotFound();
            }
            return PartialView(livro);
        }

        // GET: Livros/Create
        public ActionResult Create()
        {
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome");
            return PartialView();
        }

        // POST: Livros/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Titulo,Autor,AnoEdicao,Valor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                db.Livros.Add(livro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Livros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livro livro = db.Livros.Find(id);
            if (livro == null)
            {
                return HttpNotFound();
            }
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return PartialView(livro);
        }

        // POST: Livros/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,Autor,AnoEdicao,Valor,GeneroId")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(livro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GeneroId = new SelectList(db.Generos, "Id", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Livros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livro livro = db.Livros.Find(id);
            if (livro == null)
            {
                return HttpNotFound();
            }
            return PartialView(livro);
        }

        // POST: Livros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Livro livro = db.Livros.Find(id);
            db.Livros.Remove(livro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
