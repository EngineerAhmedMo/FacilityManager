﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildingFacilityManager.Controllers;
using BuildingFacilityManager.Models;
using BuildingFacilityManager.Models.Assets;
using BuildingFacilityManager.Models.Work_Order;
using BuildingFacilityManager.ViewModels;

namespace BuildingFacilityManager.Areas.Admin.Controllers
{
    public class AssetController : AdminAuthorizationController
    {
        private readonly ApplicationDbContext _context;

        public AssetController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _context.Dispose();
        }


        // GET: Admin/Asset
        public ActionResult Index()
        {
            var assetModel = new AssetsViewModel()
            {
                Assets = _context.Assets.Include(a => a.Space).Include(a => a.RelatedAssets).ToList(),
                Spaces = _context.Spaces.ToList()
            };
            return View(assetModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAsset(Asset asset)
        {
            var assetModel = new AssetsViewModel()
            {
                Assets = _context.Assets.Include(a => a.Space).Include(a => a.RelatedAssets).ToList(),
                Spaces = _context.Spaces.ToList()
            };

            if (asset.Label != null && asset.SpaceId != 0)
            {
                _context.Assets.Add(asset);
                _context.SaveChanges();
                var assetMod = new AssetsViewModel()
                {
                    Assets = _context.Assets.Include(a => a.Space).Include(a => a.RelatedAssets).ToList(),
                    Spaces = _context.Spaces.ToList(),

                };
                return View("Index", assetMod);
            }
            return View("Index", assetModel);
        }

        [HttpGet]
        public ActionResult AddAssetsFromSpaceAssets(int id, string name)
        {
            ViewBag.SpaceId = id;
            ViewBag.SpaceLabel = name;
            var assetModel = new AssetsViewModel()
            {
                Assets = _context.Assets.Include(a => a.Space).Include(a => a.RelatedAssets).ToList(),
                Spaces = _context.Spaces.ToList()
            };
            return View(assetModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAssetsFromSpaceAssets(Asset asset)
        {
            if (asset.Label != null)
            {
                _context.Assets.Add(asset);
                _context.SaveChanges();
            }
            var space = _context.Spaces.Include(s => s.Assets).SingleOrDefault(s => s.Id == asset.SpaceId);

            var spaceAssets = new List<Asset>();
            if (space != null)
                foreach (var ass in space.Assets)
                    spaceAssets.Add(ass);
            if (space != null) ViewBag.spaceLabel = space.Label;
            ViewBag.SpaceId = asset.SpaceId;

            return View("SpaceAssets", spaceAssets);
        }


        [HttpGet]
        public ActionResult AssetsWorkOrders(int id)
        {
            var myAsset = _context.Assets.SingleOrDefault(a => a.Id == id);
            if (myAsset != null) ViewBag.AssetLabel = myAsset.Label;
            var assetMod = new AssetsViewModel()
            {
                WorkOrders = _context.WorkOrders.Where(w => w.AssetId == id)
                    .Include(w => w.Asset.Space)
                    .Include(w => w.Asset.Space.Storey)
                    .Include(w => w.Reporter)
                    .Include(w => w.Fixer)
                    .ToList()

            };

            return PartialView("Assets_WorkOrders_Partial", assetMod);
        }

        public ActionResult SpaceAssets(int id)
        {
            var space = _context.Spaces.Include(s => s.Assets).SingleOrDefault(s => s.Id == id);

            var spaceAssets = new List<Asset>();
            if (space != null)
                foreach (var asset in space.Assets)
                    spaceAssets.Add(asset);
            if (space != null) ViewBag.spaceLabel = space.Label;
            ViewBag.SpaceId = id;

            return View(spaceAssets);
        }

        public ActionResult AssetsDetails(int id)
        {
            ViewBag.AssetId = id;
            var workModel = new WorkOrderViewModel()
            {
                Asset = _context.Assets.Include(a => a.Space)
                    .Include(a => a.RelatedAssets)
                    .Include(a => a.Space.Storey)
                    .Include(a => a.WorkOrders)
                    .SingleOrDefault(a => a.Id == id),
                WorkOrders = _context.WorkOrders.Include(w => w.Asset)
                    .Include(w => w.Asset.Space)
                    .Include(w => w.Asset.Space.Storey)
                    .ToList(),
                Assets = _context.Assets.ToList(),
                Spaces = _context.Spaces.ToList(),
            };

            return View(workModel);
        }



        public ActionResult AddWorkOrderFromAssetDetails(WorkOrder workOrder)
        {
            var workModel = new WorkOrderViewModel()
            {
                WorkOrders = _context.WorkOrders.Include(w => w.Asset)
                    .Include(w => w.Asset.Space)
                    .Include(w => w.Asset.Space.Storey)
                    .ToList(),
                Assets = _context.Assets.ToList(),
                Spaces = _context.Spaces.ToList(),
                Asset = _context.Assets.Include(a => a.Space)
                    .Include(a => a.RelatedAssets)
                    .Include(a => a.Space.Storey)
                    .Include(a => a.WorkOrders)
                    .SingleOrDefault(a => a.Id == workOrder.AssetId)
            };

            if (workOrder.Description != null && workOrder.WorkOrderStatus != 0 && workOrder.AssetId != 0)
            {
                _context.WorkOrders.Add(workOrder);
                _context.SaveChanges();
                var workMod = new WorkOrderViewModel()
                {
                    WorkOrders = _context.WorkOrders.Include(w => w.Asset)
                        .Include(w => w.Asset.Space)
                        .Include(w => w.Asset.Space.Storey)
                        .ToList(),
                    Assets = _context.Assets.ToList(),
                    Spaces = _context.Spaces.ToList(),
                    Asset = _context.Assets.Include(a => a.Space)
                        .Include(a => a.RelatedAssets)
                        .Include(a => a.Space.Storey)
                        .Include(a => a.WorkOrders)
                        .SingleOrDefault(a => a.Id == workOrder.AssetId)
                };
                return View("AssetsDetails", workMod);

            }
            return View("AssetsDetails", workModel);
        }


        [HttpGet]
        public ActionResult AddWorkOrderFromSpaceAssetsForm(int id)
        {
            ViewBag.AssetId = id;
            var workModel = new WorkOrderViewModel()
            {
                Asset = _context.Assets.Include(a => a.Space)
                    .Include(a => a.RelatedAssets)
                    .Include(a => a.Space.Storey)
                    .Include(a => a.WorkOrders)
                    .SingleOrDefault(a => a.Id == id),
                WorkOrders = _context.WorkOrders.Include(w => w.Asset)
                    .Include(w => w.Asset.Space)
                    .Include(w => w.Asset.Space.Storey)
                    .ToList(),
                Assets = _context.Assets.ToList(),
                Spaces = _context.Spaces.ToList(),
            };
            return View(workModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddWorkOrderFromSpaceAssets(WorkOrder workOrder)
        {
            var workModel = new WorkOrderViewModel()
            {
                WorkOrders = _context.WorkOrders.Include(w => w.Asset)
                    .Include(w => w.Asset.Space)
                    .Include(w => w.Asset.Space.Storey)
                    .ToList(),
                Assets = _context.Assets.ToList(),
                Spaces = _context.Spaces.ToList(),
                Asset = _context.Assets.Include(a => a.Space)
                    .Include(a => a.RelatedAssets)
                    .Include(a => a.Space.Storey)
                    .Include(a => a.WorkOrders)
                    .SingleOrDefault(a => a.Id == workOrder.AssetId)
            };

            if (workOrder.Description != null && workOrder.WorkOrderStatus != 0 && workOrder.AssetId != 0)
            {
                _context.WorkOrders.Add(workOrder);
                _context.SaveChanges();
                var workMod = new WorkOrderViewModel()
                {
                    WorkOrders = _context.WorkOrders.Include(w => w.Asset)
                        .Include(w => w.Asset.Space)
                        .Include(w => w.Asset.Space.Storey)
                        .ToList(),
                    Assets = _context.Assets.ToList(),
                    Spaces = _context.Spaces.ToList(),
                    Asset = _context.Assets.Include(a => a.Space)
                        .Include(a => a.RelatedAssets)
                        .Include(a => a.Space.Storey)
                        .Include(a => a.WorkOrders)
                        .SingleOrDefault(a => a.Id == workOrder.AssetId)
                };
                return View("AssetsDetails", workMod);

            }
            return View("AssetsDetails", workModel);
        }





        /// <summary>
        ///  Deprecated, since i used a plugin called Datatables
        /// </summary>

        //public ActionResult SpaceAssetsSort(string sortOrder, int id)
        //{
        //    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    ViewBag.IdSortParm = !string.IsNullOrEmpty(sortOrder) ? "id_sort" : "";
        //    ViewBag.StatusSortParm = !string.IsNullOrEmpty(sortOrder) ? "status_sort" : "";
        //    ViewBag.TypeSortParm = !string.IsNullOrEmpty(sortOrder) ? "Type_sort" : "";


        //    var space = _context.Spaces.Include(s => s.Assets).SingleOrDefault(s => s.Id == id);
        //    var spaceAssets = new List<Asset>();
        //    if (space != null)
        //        foreach (var asset in space.Assets)
        //            spaceAssets.Add(asset);


        //    spaceAssets.Sort((a, b) => String.Compare(a.Label, b.Label, StringComparison.Ordinal));


        //    if (space != null) ViewBag.spaceLabel = space.Label;
        //    ViewBag.SpaceId = id;

        //    return View("SpaceAssets", spaceAssets); // return the View of the SpaceAssets Action
        //}
    }
}