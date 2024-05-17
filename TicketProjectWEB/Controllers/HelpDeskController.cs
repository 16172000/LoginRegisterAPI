using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketProjectWEB.Models;

namespace TicketProjectWEB.Controllers
{
    [SessionAuthorize]
    public class HelpDeskController : Controller
    {
        private readonly CoreProject5DbContext _context;

        public HelpDeskController(CoreProject5DbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var coreProject5DbContext = _context.Ticketts.Include(t => t.CreatedByNavigation);
        //    return View(await coreProject5DbContext.ToListAsync());
        //}


        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name;
            var coreProject5DbContext = _context.Ticketts.Include(t => t.CreatedByNavigation);

            if (userName == "Admin")
            {
                return View(await coreProject5DbContext.ToListAsync());
            }
            else
            {
                var currentUserTickets = coreProject5DbContext.Where(t => t.CreatedByNavigation.UserName == userName);
                return View(await currentUserTickets.ToListAsync());
            }
        }





        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ticketts == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticketts
                .Include(t => t.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }



        public IActionResult Create()
        {
            string currentUserEmail = User.Identity.Name;

            var currentUser = _context.Registers.SingleOrDefault(u => u.UserName == currentUserEmail);

            if (currentUser != null)
            {
                ViewBag.CreatedBy = new SelectList(new List<string> { currentUser.Id.ToString() });
            }
            else
            {
                ViewBag.CreatedBy = new SelectList(new List<string>());
            }

            ViewBag.AssignedTo = new SelectList(_context.DepartmentTbls, "UserName", "UserName");
            ViewBag.Category = new SelectList(_context.DepartmentTbls, "Department", "Department");
            ViewBag.SubCategory = new SelectList(_context.SubCategories, "Name", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Description,Category,CreatedBy,SubCategory,Status,AssignedTo,FileUpload")] Tickett ticket)
        {
            string currentUserEmail = User.Identity.Name;

            var currentUser = _context.Registers.SingleOrDefault(u => u.UserName == currentUserEmail);

            if (currentUser != null)
            {
                ticket.CreatedBy = currentUser.Id;
            }
            else
            {
                return NotFound();
            }

            ViewBag.AssignedTo = new SelectList(_context.DepartmentTbls, "UserName", "UserName", ticket.AssignedTo);
            ViewBag.Category = new SelectList(_context.DepartmentTbls, "Department", "Department", ticket.Category);
            ViewBag.SubCategory = new SelectList(_context.SubCategories, "Name", "Name");

            if (ModelState.IsValid)
            {
                if (ticket.FileUpload != null && ticket.FileUpload.Length > 0)
                {
                    var fileName = Path.GetFileName(ticket.FileUpload.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadFile", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ticket.FileUpload.CopyToAsync(fileStream);
                    }

                    ticket.Attachment = fileName;
                }


                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ticketts == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticketts.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            //ViewData["ExistingAttachment"] = ticket.Attachment;
            ViewData["ExistingCreationDate"] = ticket.CreationDate;

            ViewBag.CreatedBy = new SelectList(_context.Registers, "Id", "Id", ticket.CreatedBy);
            ViewBag.AssignedTo = new SelectList(_context.DepartmentTbls, "UserName", "UserName");
            ViewBag.Category = new SelectList(_context.DepartmentTbls, "Department", "Department");
            ViewBag.SubCategory = new SelectList(_context.SubCategories, "Name", "Name");
            return View(ticket);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,Title,Description,Category,SubCategory,Attachment,Status,CreatedBy,CreationDate,AssignedTo,FileUpload")] Tickett ticket)
        {
            if (id != ticket.TicketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Tickett existingTicket = _context.Ticketts.Find(id);
                    if (existingTicket == null)
                    {
                        return NotFound();
                    }

                    existingTicket.Title = ticket.Title;
                    existingTicket.Description = ticket.Description;
                    existingTicket.Category = ticket.Category;
                    existingTicket.SubCategory = ticket.SubCategory;
                    existingTicket.Status = ticket.Status;
                    //existingTicket.Attachment = ticket.Attachment;
                    existingTicket.CreatedBy = ticket.CreatedBy;
                    existingTicket.AssignedTo = ticket.AssignedTo;

                    if (ticket.FileUpload != null && ticket.FileUpload.Length > 0)
                    {
                        var fileName = Path.GetFileName(ticket.FileUpload.FileName);

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadFile", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ticket.FileUpload.CopyToAsync(fileStream);
                        }

                        existingTicket.Attachment = fileName;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.TicketId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CreatedBy = new SelectList(_context.Registers, "Id", "Id", ticket.CreatedBy);
            ViewBag.AssignedTo = new SelectList(_context.DepartmentTbls, "UserName", "UserName", ticket.AssignedTo);
            ViewBag.Category = new SelectList(_context.DepartmentTbls, "Department", "Department", ticket.Category);
            ViewBag.SubCategory = new SelectList(_context.SubCategories, "Name", "Name", ticket.SubCategory);
            return View(ticket);
        }



        // GET: HelpDesk/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ticketts == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticketts
                .Include(t => t.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: HelpDesk/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ticketts == null)
            {
                return Problem("Entity set 'CoreProject5DbContext.Tickets'  is null.");
            }
            var ticket = await _context.Ticketts.FindAsync(id);
            if (ticket != null)
            {
                _context.Ticketts.Remove(ticket);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
          return (_context.Ticketts?.Any(e => e.TicketId == id)).GetValueOrDefault();
        }
    }
}

