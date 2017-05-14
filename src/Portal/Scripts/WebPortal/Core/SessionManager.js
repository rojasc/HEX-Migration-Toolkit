Microsoft.WebPortal.Core.SessionManager = function (webPortal) {
    /// <summary>
    /// Stores session information.
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>

    this.webPortal = webPortal;

    // Shell, please let us know when you have finished initializing
    this.webPortal.EventSystem.subscribe(Microsoft.WebPortal.Event.PortalInitializing, this.initialize, this);

    this.webPortal.EventSystem.subscribe(Microsoft.WebPortal.Event.FeatureDeactivated, this.onFeatureDeactivated, this);

    // a hashtable that caches the HTML template for each feature
    this.featureTemplates = {};
};

Microsoft.WebPortal.Core.SessionManager.prototype.initialize = function (eventId, context, broadcaster) {
    /// <summary>
    /// Called when the portal is initializing.
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="context"></param>
    /// <param name="broadcaster"></param>

    // assign feature presenters
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.Home, Microsoft.WebPortal.HomePagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.EnvironmentAddNew, Microsoft.WebPortal.EnvironmentAddNewPagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.MigrationBatchAddNew, Microsoft.WebPortal.MigrationBatchAddNewPagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.MigrationBatch, Microsoft.WebPortal.MigrationBatchPagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.MigrationBatches, Microsoft.WebPortal.MigrationBatchesPagePresenter);
};

Microsoft.WebPortal.Core.SessionManager.prototype.onFeatureDeactivated = function () {
    /// <summary>
    /// Called whenever a feature is deactivated.
    /// </summary>

    // clear out the actions bar
    this.webPortal.Services.Actions.clear();
};

//@ sourceURL=SessionManager.js