﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuildingFacilityManager.Models.Assets;
using BuildingFacilityManager.Models.Tasks;
using BuildingFacilityManager.Models.Work_Order;

namespace BuildingFacilityManager.ViewModels
{
    public class DashBoardViewModel
    {
        public ICollection<WorkOrder> WorkOrders { get; set; }
        public ICollection<WorkOrder> TodayCompletedWorkOrders { get; set; }
        public ICollection<WorkOrder> TodayActiveWorkOrders { get; set; }
        public ICollection<WorkOrder> TodayInProgressWorkOrders { get; set; }
        public ICollection<WorkOrder> MyCreatedWorkOrdersToday { get; set; }
        public ICollection<Asset> Assets { get; set; }

        public ICollection<InspectionTask> TodayInspectionTasks { get; set; }
        public ICollection<InspectionTask> ScheduledInspectionTasks { get; set; }
        public InspectionTask InspectionTask { get; set; }
    }
}