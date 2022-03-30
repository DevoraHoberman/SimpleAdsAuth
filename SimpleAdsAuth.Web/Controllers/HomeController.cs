using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdsAuth;Integrated Security=true;";

        public IActionResult Index()
        {
            var repo = new SimpleAdsAuthRepo(_connectionString);
            var vm = new IndexViewModel()
            {
                Ads = repo.GetAds(),
                IsAuthenticated = User.Identity.IsAuthenticated
            };
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                vm.CurrentUser = repo.GetByEmail(email);
            }
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var repo = new SimpleAdsAuthRepo(_connectionString);
            var isAuthenticted = User.Identity.IsAuthenticated;
            if (!isAuthenticted)
            {
                return Redirect("/account/login");
            }
            var user = repo.GetByEmail(User.Identity.Name);
            repo.NewAd(ad, user.Id);

            return Redirect("/");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new SimpleAdsAuthRepo(_connectionString);
            var user = repo.GetByEmail(User.Identity.Name);
            return View(new MyAdsViewModel
            {
                MyAds = repo.MyAds(user.Id)
            });

        }

        [Authorize]
        public IActionResult DeleteAd(int id)
        {
            var repo = new SimpleAdsAuthRepo(_connectionString);
            repo.DeleteAd(id);
            return Redirect("/");
        }
    }
}
