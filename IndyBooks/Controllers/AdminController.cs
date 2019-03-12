using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDataContext _db;
        public AdminController(IndyBooksDataContext db) { _db = db; }
        
        /***
         * CREATE
         */
        [HttpGet]
        public IActionResult CreateBook()
        {
            AddBookViewModel AddVM = new AddBookViewModel();
            AddVM.WritersList = _db.Writers.ToList();
            return View("AddBook", AddVM);
        }
        [HttpPost]
        public IActionResult CreateBook(AddBookViewModel newBook)
        {
            //TODO: Build the Author and the Book given the newBook data. Add to DbSets; SaveChanges

            Writer author = new Writer();
            Book book = new Book();

            if (newBook.Name != null)
            {
                author.Name = newBook.Name;
            }
            else
            {
                author = _db.Writers.Single(w => w.Id == newBook.AuthorId);
            }
            book.Author = author;
            book.Price = newBook.Price;
            book.SKU = newBook.SKU;
            book.Title = newBook.Title;
            _db.Books.Add(book);
            _db.SaveChanges();



            //Shows the new book using the Search Listing 
            return RedirectToAction("Index");
        }
        /***
         * READ       
         */
        [HttpGet]
        public IActionResult Index()
        {
            //TODO: Use lambda methods as described by the variable name
            var allBooksWithAuthorsOrderedbySKU = _db.Books.Include(a => a.Author).OrderBy(s => s.SKU);
            return View("SearchResults", allBooksWithAuthorsOrderedbySKU);
        }
        /***
         * UPDATE
         */
         //TODO: BONUS - Write an method that take a book id, and loads the book data in to the AddBook View

        /***
         * DELETE
         */
        [HttpGet]
        public IActionResult DeleteBook(long id)
        {
            //TODO: Remove the Book associated with the given id number; Save Changes
            _db.Books.Remove(new Book { Id = id });
            _db.SaveChanges();

            return RedirectToAction("Search");
        }

        [HttpGet]
        public IActionResult Search() { return View(); }
        [HttpPost]
        public IActionResult Search(SearchViewModel search)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books; // start with entire collection

            //Partial Title Search
            if (search.Title != null)
            {
                foundBooks = foundBooks
                            .Where(b => b.Title.Contains(search.Title))
                            .OrderBy(b => b.Title)
                            ;
            }

            //Author's Last Name Search
            if (search.AuthorLastName != null)
            {
                //Use the Name property of the Book's Author entity
                foundBooks = foundBooks
                            .Include(b => b.Author)
                            .Where(b => b.Author.Name.EndsWith(search.AuthorLastName, StringComparison.CurrentCulture))
                            ;
            }
            //Priced Between Search (min and max price entered)
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                            .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice)
                            .Select(b => new Book { Author = b.Author })
                            .Distinct()
                            ;
            }
            //Highest Priced Book Search (only max price entered)
            if (search.MinPrice == 0 && search.MaxPrice > 0)
            {
                decimal max = _db.Books.Max(b => b.Price);
                foundBooks = foundBooks
                            .Where(b => b.Price == max)
                            ;
            }
            //Composite Search Results
            return View("SearchResults", foundBooks.Include(b => b.Author));
        }

    }
}
