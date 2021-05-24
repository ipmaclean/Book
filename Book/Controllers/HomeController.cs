using Book.Models;
using Google.Apis.Books.v1.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Book.Controllers
{
    public class HomeController : Controller
    {
        dbBookEntities db = new dbBookEntities();

        [AllowAnonymous]
        public async Task<ActionResult> Index(string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {

                return View(await BookSearch.Search(searchTerm, 40, 0));
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult MyBooks(string ReadValue)
        {
            List<string> readList = new List<string>() { "Read", "Unread" };

            ViewBag.ReadValue = new SelectList(readList);

            string userId = User.Identity.GetUserId();

            // select * from tbSavedBooks; display all books that match the UserId
            var books = from b in db.tbSavedBooks
                        select b;
            books = books.Where(b => b.UserNameId == userId);

            if (ReadValue != null)
            {
                
                if (ReadValue == "Read")
                {
                    bool readStatus = true;
                    books = books.Where(b => b.Read == readStatus);
                }
                else if (ReadValue == "Unread")
                {
                    bool readStatus = false;
                    books = books.Where(b => b.Read == readStatus);
                }
            }

            ViewBag.Message = "Your saved books.";

            

            return View(books);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Create(string BookId)
        {
            if (ModelState.IsValid)
            {
                var BookData = await BookSearch.Search(BookId);

                var book = new tbSavedBooks();
                book.Title = BookData.VolumeInfo.Title;
                book.Author = (BookData.VolumeInfo.Authors != null) ? BookData.VolumeInfo.Authors.FirstOrDefault() : "Author not found.";
                book.Description = (BookData.VolumeInfo.Description != null) ? BookData.VolumeInfo.Description : "Description not found.";
                book.Image = BookData.VolumeInfo.ImageLinks.Thumbnail;
                book.Read = false;
                book.UserNameId = User.Identity.GetUserId();

                return View(book);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Author,Description,Image,Review,Read,UserNameId")] tbSavedBooks model)
        {
            if (ModelState.IsValid)
            {
                db.tbSavedBooks.Add(model);
                db.SaveChanges();

                return RedirectToAction("MyBooks");
            }
            else
            {
                return Content("Sorry, something went wrong.");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                tbSavedBooks book = db.tbSavedBooks.Find(Id);
                if (book == null)
                {
                    return HttpNotFound();
                }
                else if (book.UserNameId != User.Identity.GetUserId())
                {
                    return new HttpUnauthorizedResult();
                }
                else
                {
                    return View(book);
                }
            }
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include = "Id,Title,Author,Description,Image,Review,Read,UserNameId")] int Id)
        {
            tbSavedBooks book = db.tbSavedBooks.Find(Id);

            if (book == null)
            {
                return HttpNotFound();
            }
            else if (book.UserNameId != User.Identity.GetUserId())
            {
                return new HttpUnauthorizedResult();
            }
            else
            {
                db.tbSavedBooks.Remove(book);
                db.SaveChanges();
                return RedirectToAction("MyBooks");
            }

            
        }

        [Authorize]
        [HttpGet]
        public ActionResult Review(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                // select * from moview where Id = x
                tbSavedBooks book = db.tbSavedBooks.Find(Id);
                if (book == null)
                {
                    return HttpNotFound();
                }
                else if (book.UserNameId != User.Identity.GetUserId())
                {
                    return new HttpUnauthorizedResult();
                }
                else
                {
                    return View(book);
                }
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Review([Bind(Include = "Id,Title,Author,Description,Image,Review,Read,UserNameId")] tbSavedBooks book)
        {
            if (ModelState.IsValid && book.UserNameId == User.Identity.GetUserId())
            {
                // Modify database
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyBooks");
            }
            else
            {

                return Content("Sorry, something went wrong.");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeRead([Bind(Include = "Id,Title,Author,Description,Image,Review,Read,UserNameId")] tbSavedBooks book)
        {
            if (ModelState.IsValid && book.UserNameId == User.Identity.GetUserId())
            {
                if (book.Read)
                {
                    book.Read = false;
                }
                else
                {
                    book.Read = true;
                }
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("MyBooks");
            }
            else
            {
                return Content("Sorry, something went wrong.");
            }
        }
    }
}