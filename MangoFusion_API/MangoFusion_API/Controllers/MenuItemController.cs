using MangoFusion_API.Data;
using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Net;

namespace MangoFusion_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : Controller
    {
        // Dependency Injection

        private readonly ApplicationDbContext _db;  // It allows your controller to query or update the database (e.g., _db.MenuItems.ToList(),

        private readonly ApiResponse _response; // It standardizes the structure of API responses, making it easier to handle success and error messages consistently.

        private readonly IWebHostEnvironment _env;  // It provides information about the web hosting environment, such as the content root path and web root path, which can be useful for file handling and environment-specific configurations.


        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _response = new ApiResponse();

            _env = env;
        }

        [HttpGet]
        public IActionResult GetMenuItems()
        {
            _response.Result = _db.MenuItems.ToList();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }

        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public IActionResult GetMenuItems(int id)
        {
            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage.Add("Invalid ID");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            MenuItem? menuItem = _db.MenuItems.FirstOrDefault(u => u.Id == id);
            _response.Result = menuItem;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }



        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string? imageFileName = null;


                    if (menuItemCreateDTO.File != null && menuItemCreateDTO.File.Length > 0)
                    {
                        var imgPath = Path.Combine(_env.WebRootPath, "images");
                        if (!Directory.Exists(imgPath))
                        {
                            Directory.CreateDirectory(imgPath);
                        }
                        var imagePath = Path.Combine(imgPath, menuItemCreateDTO.File.FileName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                            //  _response.IsSuccess = false;
                            // _response.ErrorMessage.Add("Image with the same name already exists.");
                            //  _response.StatusCode = HttpStatusCode.BadRequest;
                            //  return BadRequest(_response);

                        }
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await menuItemCreateDTO.File.CopyToAsync(stream);
                        }

                        imageFileName = menuItemCreateDTO.File.FileName;


                        //_response.IsSuccess = false;
                        //_response.ErrorMessage.Add("Image file is required.");
                        //_response.StatusCode = HttpStatusCode.BadRequest;
                        //return BadRequest(_response);
                    }

                    // Upload the file to the server


                    MenuItem menuItem = new()
                    {
                        Name = menuItemCreateDTO.Name,
                        Description = menuItemCreateDTO.Description,
                        Category = menuItemCreateDTO.Category,
                        Price = menuItemCreateDTO.Price,
                        SpecialTag = menuItemCreateDTO.SpecialTag,
                        Image = "images/" + imageFileName
                    };

                    _db.MenuItems.Add(menuItem);
                    await _db.SaveChangesAsync();

                    _response.Result = menuItem;
                    _response.StatusCode = HttpStatusCode.Created;
                    //  _response.ErrorMessage.Add("Image file is added.");
                    return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, _response);



                }
                else
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage.Add("Invalid model state.");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { ex.ToString() };

                //   _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);

            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //  string? imageFileName = null;


                    if (id == 0)
                    {
                        _response.ErrorMessage.Add(".invalid data");
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_response);

                    }
                    MenuItem? menuItemFromDb = _db.MenuItems.FirstOrDefault(u => u.Id == id);

                    if (menuItemFromDb == null)
                    {
                        _response.ErrorMessage.Add("Menu item not found.");
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }



                    // ✅ Delete image file if it exists
                    if (!string.IsNullOrEmpty(menuItemFromDb.Image))
                    {
                        var oldFilePath = Path.Combine(_env.WebRootPath, menuItemFromDb.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }


                    _db.MenuItems.Remove(menuItemFromDb);
                    await _db.SaveChangesAsync();

                    //       _response.Result = menuItemFromDb;
                    _response.StatusCode = HttpStatusCode.NoContent;
                    //  _response.ErrorMessage.Add("Image file is added.");
                    return Ok(_response);



                }
                else
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage.Add("Invalid model state.");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { ex.ToString() };

                //   _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);

            }
        }



        [HttpPut]
        public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (menuItemUpdateDTO == null || menuItemUpdateDTO.Id != id)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessage.Add("Invalid data.");
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_response);
                    }
                    MenuItem menuItemFromDb = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);
                    if (menuItemFromDb == null)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessage.Add("Menu item not found.");
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }


                    // ✅ Update basic fields
                    menuItemFromDb.Name = menuItemUpdateDTO.Name;
                    menuItemFromDb.Description = menuItemUpdateDTO.Description;
                    menuItemFromDb.Price = menuItemUpdateDTO.Price;
                    menuItemFromDb.Category = menuItemUpdateDTO.Category;
                    menuItemFromDb.SpecialTag = menuItemUpdateDTO.SpecialTag;



                    if (menuItemUpdateDTO.File != null && menuItemUpdateDTO.File.Length > 0)
                    {
                        var imgPath = Path.Combine(_env.WebRootPath, "images");
                        if (!Directory.Exists(imgPath))
                        {
                            Directory.CreateDirectory(imgPath);
                        }
                        var imagePath = Path.Combine(imgPath, menuItemUpdateDTO.File.FileName);
                        // ✅ If old image exists, delete it safely
                        if (!string.IsNullOrEmpty(menuItemFromDb.Image))
                        {
                            var oldFilePath = Path.Combine(_env.WebRootPath, menuItemFromDb.Image);
                            if (System.IO.File.Exists(oldFilePath))
                            { 
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        // ✅ Save new image
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await menuItemUpdateDTO.File.CopyToAsync(stream);
                        }

                        menuItemFromDb.Image = "images/" + menuItemUpdateDTO.File.FileName;


                        //_response.IsSuccess = false;
                        //_response.ErrorMessage.Add("Image file is required.");
                        //_response.StatusCode = HttpStatusCode.BadRequest;
                        //return BadRequest(_response);
                    }else
                    {
                        // If no new file is provided, retain the existing image path
                        //  menuItemFromDb.Image = menuItemFromDb.Image;

                        // ✅ Optional: allow removing image without uploading a new one
                        if (!string.IsNullOrEmpty(menuItemFromDb.Image))
                        {
                            var oldImagePath = Path.Combine(_env.WebRootPath, menuItemFromDb.Image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        menuItemFromDb.Image = null;

                    }

                    // Upload the file to the server


                    //MenuItem menuItem = new()
                    //{
                    //    Name = menuItemCreateDTO.Name,
                    //    Description = menuItemCreateDTO.Description,
                    //    Category = menuItemCreateDTO.Category,
                    //    Price = menuItemCreateDTO.Price,
                    //    SpecialTag = menuItemCreateDTO.SpecialTag,
                    //    Image = "images/" + imageFileName
                    //};

                    _db.MenuItems.Update(menuItemFromDb);
                    await _db.SaveChangesAsync();

                    _response.Result = menuItemFromDb;
                    _response.StatusCode = HttpStatusCode.Created;
                    //  _response.ErrorMessage.Add("Image file is added.");
                    return Ok(_response);



                }
                else
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage.Add("Invalid model state.");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { ex.ToString() };

                //   _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);

            }
        }














    }

}








