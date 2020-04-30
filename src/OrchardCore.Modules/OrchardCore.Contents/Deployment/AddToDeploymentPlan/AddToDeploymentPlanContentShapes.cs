using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Deployment.Services;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.Entities;
using OrchardCore.Modules;
using OrchardCore.Settings;

namespace OrchardCore.Contents.Deployment.AddToDeploymentPlan
{
    [Feature("OrchardCore.Contents.AddToDeploymentPlan")]
    public class AddToDeploymentPlanContentShapes : IShapeTableProvider
    {
        private readonly IDeploymentPlanService _deploymentPlanService;

        public AddToDeploymentPlanContentShapes(
            IDeploymentPlanService deploymentPlanService
            )
        {
            _deploymentPlanService = deploymentPlanService;
        }

        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("Contents_SummaryAdmin__Button__Actions")
                .OnDisplaying(async displaying =>
                {
                    if (await _deploymentPlanService.DoesUserHavePermissionsAsync())
                    {
                        displaying.Shape.Metadata.Wrappers.Add("AddToDeploymentPlan_Wrapper__ActionDeploymentPlan");
                    }
                });

            builder.Describe("AdminBulkActions")
                .OnDisplaying(async context =>
                {
                    if (await _deploymentPlanService.DoesUserHavePermissionsAsync())
                    {
                        dynamic shape = context.Shape;
                        var shapeFactory = context.ServiceProvider.GetRequiredService<IShapeFactory>();
                        var bulkActionsShape = await shapeFactory.CreateAsync("AddToDeploymentPlan__Button__ContentsBulkAction");
                        // Don't use Items.Add() or the collection won't be sorted
                        shape.Add(bulkActionsShape, ":20");

                        var addToDeploymentPlanModal = await shapeFactory.CreateAsync("AddToDeploymentPlan_Modal__ContentsBulkActionDeploymentPlan");
                        shape.Add(addToDeploymentPlanModal);
                    }
                });
        }
    }
}