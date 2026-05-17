using BlogStore.Data;
using BlogStore.Models;
using BlogStore.Models.ViewModels;
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

        public JsonResult AddComment([FromBody]Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return Json(new
            {
                username=comment.UserName,
                commentDate=comment.CommentDate.ToString("MMMM dd, yyyy"),
                content=comment.Content
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
