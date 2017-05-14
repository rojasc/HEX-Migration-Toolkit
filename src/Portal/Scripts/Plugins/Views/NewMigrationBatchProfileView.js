Microsoft.WebPortal.Views.NewMigrationBatchProfileView = function (webPortal, elementSelector, isShown, animation) {
    /// <summary>
    /// A view that render input fields for a new migration batch.
    /// </summary>
    /// <param name="webPortal">The web portal instance</param>
    /// <param name="elementSelector">The JQuery selector for the HTML element this view will own.</param>
    /// <param name="isShown">The initial show state. Optional. Default is false.</param>
    /// <param name="animation">Optional animation to use for showing and hiding the view.</param>

    this.base.constructor.call(this, webPortal, elementSelector, isShown, null, animation);
    this.template = "newMigrationBatchProfile-template";

    this.viewModel = {
        Id: ko.observable(""),
        Name: ko.observable(""),
        StartTime: ko.observable("")
    };
}

// extend the base view
$WebPortal.Helpers.inherit(Microsoft.WebPortal.Views.NewMigrationBatchProfileView, Microsoft.WebPortal.Core.View);

Microsoft.WebPortal.Views.NewMigrationBatchProfileView.prototype.onRender = function () {
    /// <summary>
    /// Called when the view is rendered.
    /// </summary>

    $(this.elementSelector).attr("data-bind", "template: { name: '" + this.template + "'}");
    ko.applyBindings(this, $(this.elementSelector)[0]);
}

Microsoft.WebPortal.Views.NewMigrationBatchProfileView.prototype.onShowing = function (isShowing) {
}

Microsoft.WebPortal.Views.NewMigrationBatchProfileView.prototype.onShown = function (isShowing) {
}

Microsoft.WebPortal.Views.NewMigrationBatchProfileView.prototype.onDestroy = function () {
    /// <summary>
    /// Called when the journey trail is about to be destroyed.
    /// </summary>

    if ($(this.elementSelector)[0]) {
        // if the element is there, clear its bindings and clean up its content
        ko.cleanNode($(this.elementSelector)[0]);
        $(this.elementSelector).empty();
    }
}

//@ sourceURL=NewMigrationBatchProfileView.js