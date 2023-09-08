using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;
using RunGroupWebApp.Services;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor; 
        }
        public async Task<IActionResult> Index()//ccc Controller
        {
            IEnumerable<Races> races = await _raceRepository.GetAll();//mmm Model
            return View(races);//vvv view
        }


        public async Task<IActionResult> Detail(int id)
        {
            Races races = await _raceRepository.GetByIdAsync(id);
            return View(races);
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    var curUserID = _httpContextAccessor.HttpContext?.User.GetUserId();
        //    var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserID };
        //    return View(createRaceViewModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _photoService.AddPhotoAsync(raceVM.Image);


        //        var race = new Races
        //        {
        //            Title = raceVM.Title,
        //            Description = raceVM.Description,
        //            Image = result.Url.ToString(),
        //            AppUserId= raceVM.AppUserId,
        //            Address = new Address
        //            {
        //                Street = raceVM.Address.Street,
        //                City = raceVM.Address.City,
        //                State = raceVM.Address.State
        //            }
        //        };

        //        _raceRepository.Add(race);
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Photo upload Failed");
        //    }
        //    return View(raceVM);
        //}

        [HttpGet]
        public IActionResult Create()
        {
            var curUserId = HttpContext.User.GetUserId();
            var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserId };
            return View(createRaceViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);


                var race = new Races
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = result.Url.ToString(),
                    AppUserId = raceVM.AppUserId,
                    Address = new Address
                    {
                        Street = raceVM.Address.Street,
                        City = raceVM.Address.City,
                        State = raceVM.Address.State
                    }
                };

                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload Failed");
            }
            return View(raceVM);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if(race == null)return View("Error");
            var raceVM = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image.ToString(),
                RaceCategory = race.RaceCategory
            };
            return View(raceVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", raceVM);
            }

            var userClub = await _raceRepository.GetByIdAsyncNoTracking(id);

            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Could not delete photo and error code is : {ex}");
                    return View(raceVM);
                }

                var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

                //if (photoResult.Error != null)
                //{
                //    ModelState.AddModelError("Image", "Photo upload failed");
                //    return View(clubVM);
                //}

                //if (!string.IsNullOrEmpty(userClub.Image))
                //{
                //var photoResults = _photoService.DeletePhotoAsync(userClub.Image);
                //}

                var races = new Races
                {
                    Id = id,
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = raceVM.AddressId,
                    Address = raceVM.Address,
                };

                _raceRepository.Update(races);

                return RedirectToAction("Index");
            }
            else
            {
                return View(raceVM); 
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var raceDetails = await _raceRepository.GetByIdAsync(id);
            if (raceDetails == null) return View("Error");
            return View(raceDetails);
        }
        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteRace(int id)
        {
            var raceDetails = await _raceRepository.GetByIdAsync(id);
            if (raceDetails == null) return View("Error");
            _raceRepository.Delete(raceDetails);
            return RedirectToAction("Index");
        }
    }
}
