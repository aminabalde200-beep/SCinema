using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SCinema.Data;
using SCinema.Models;

namespace SCinema.Controllers
{
    [Authorize(Roles = "Fournisseur")]
    public class SallesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SallesController(ApplicationDbContext context) => _context = context;

        // ... garde tout le CRUD tel qu’il a été scaffoldé ...
    }
}
