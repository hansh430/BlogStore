using BlogStore.Data;
using BlogStore.Models;
using BlogStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogStore.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] alowedExtensions = { ".jpg", ".jpeg", ".png" };
        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(int? categoryId)
        {
            var postQuery = _context.Posts.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryId);
            }
            var posts = postQuery.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(posts);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
                return NotFound();
            var post = _context.Posts.Include(p => p.Category).Include(p => p.Comments).FirstOrDefault(p => p.Id == id);
            if (post == null)
                return NotFound();
            return View(post);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PostViewModel pvModel = new PostViewModel();
            pvModel.Categories = _context.Categories.Select(c =>
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(pvModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PostViewModel pvModel)
        {
            if (ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(pvModel.FeatureImage.FileName).ToLower();
                bool isAllowed = alowedExtensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid image format,allowed only .jpg, jpeg, .png");
                    return View(pvModel);
                }
                pvModel.Post.FeatureImagePath = await UploadFileToFolder(pvModel.FeatureImage);
                await _context.Posts.AddAsync(pvModel.Post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            pvModel.Categories = _context.Categories.Select(c =>
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(pvModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
                return NotFound();
            var postFromDB = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (postFromDB == null)
                return NotFound();
            EditViewModel editViewModel = new EditViewModel
            {
                Post = postFromDB,
                Categories = _context.Categories.Select(c =>
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(editViewModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditViewModel editViewModel)
        {
            if (!ModelState.IsValid)
                return View(editViewModel);
            var postFromDB = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == editViewModel.Post.Id);
            if (postFromDB == null)
                return NotFound();
            if (editViewModel.FeatureImage != null)
            {

                var inputFileExtension = Path.GetExtension(editViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = alowedExtensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid image format,allowed only .jpg, jpeg, .png");
                    return View(editViewModel);
                }
                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath,
                    "Images", Path.GetFileName(postFromDB.FeatureImagePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
                editViewModel.Post.FeatureImagePath = await UploadFileToFolder(editViewModel.FeatureImage);
            }
            else
            {
                editViewModel.Post.FeatureImagePath = postFromDB.FeatureImagePath;
            }
            _context.Posts.Update(editViewModel.Post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var postFromDb = await _context.Posts.FindAsync(id);
            if (postFromDb == null)
            {
                return NotFound();
            }
            return View(postFromDb);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var postFromDb = await _context.Posts.FindAsync(id);
            if (postFromDb == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(postFromDb.FeatureImagePath))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(postFromDb.FeatureImagePath));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Posts.Remove(postFromDb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public JsonResult AddComment([FromBody] Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return Json(new
            {
                username = comment.UserName,
                commentDate = comment.CommentDate.ToString("MMMM dd, yyyy"),
                content = comment.Content
            });
        }
        private async Task<string> UploadFileToFolder(IFormFile featureImage)
        {
            var inputFileExtension = Path.GetExtension(featureImage.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var imageFolderPath = Path.Combine(wwwRootPath, "images");
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }
            var filePath = Path.Combine(imageFolderPath, fileName);
            try
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await featureImage.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return "Error Uploading images: " + ex.Message;
            }
            return "/images/" + fileName;
        }

    }
}
