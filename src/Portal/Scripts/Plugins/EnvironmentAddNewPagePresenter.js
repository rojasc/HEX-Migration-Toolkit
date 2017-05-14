Microsoft.WebPortal.EnvironmentAddNewPagePresenter = function (webPortal, feature, context) {
    /// <summary>
    /// Provides a mechanism for adding new environments. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    this.base.constructor.call(this, webPortal, feature, "Home", "/Template/EnvironmentAddNew/");

    this.environmentProfileView = new Microsoft.WebPortal.Views.NewEnvironmentProfileView(webPortal, "#EnvironmentProfileContainer");

    this.context = context;
    this.environmentRegistrationInfo;
    this.isPosting = false;

    var self = this;

    this.onFormSubmit = function () {
        if (self.isPosting) {
            return;
        }

        if ($("#Form").valid()) {
            self.isPosting = true;
            var environmentNotification;

            environmentNotification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Progress, self.webPortal.Resources.Strings.Plugins.EnvironmentAddNew.EnvironmentRegistrationMessage);
            self.webPortal.Services.Notifications.add(environmentNotification);

            new Microsoft.WebPortal.Utilities.RetryableServerCall(self.webPortal.Helpers.ajaxCall("api/environment",
                Microsoft.WebPortal.HttpMethod.Post,
                self.getEnvironmentInformation(),
                Microsoft.WebPortal.ContentType.Json, 120000),
                "create", []).execute()
                // Success of create environment API call. 
                .done(function (registrationConfirmation) {
                    self.environmentProfileView.viewModel.Id(registrationConfirmation.Id);
                    self.environmentRegistrationInfo = registrationConfirmation; // maintain this for future retries while user is still on the same page. 

                    // turn the notification into a success
                    environmentNotification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Success);
                    notificationMessage = self.webPortal.Resources.Strings.Plugins.EnvironmentAddNew.EnvironmentRegistrationSuccessMessage + " - " + registrationConfirmation.Name + " (" + registrationConfirmation.Id + ")";
                    environmentNotification.message(notificationMessage);
                    environmentNotification.buttons([
                        Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                            environmentNotification.dismiss();
                        })
                    ]);

                    self.webPortal.Journey.start(Microsoft.WebPortal.Feature.Home);
                })
                // Failure of create environment API call. 
                .fail(function (result, status, error) {
                    self.environmentProfileView.viewModel.Id(""); // we want this clear so that create environment call can be retried by user. 

                    environmentNotification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Error);
                    environmentNotification.buttons([
                        Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                            environmentNotification.dismiss();
                        })
                    ]);

                    var errorPayload = JSON.parse(result.responseText);

                    if (errorPayload) {
                        switch (errorPayload.ErrorCode) {
                            case Microsoft.WebPortal.ErrorCode.InvalidAddress:
                                environmentNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.InvalidAddress);
                                break;
                            case Microsoft.WebPortal.ErrorCode.DomainNotAvailable:
                                environmentNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.DomainNotAvailable);
                                break;
                            case Microsoft.WebPortal.ErrorCode.InvalidInput:
                                environmentNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.InvalidInputErrorPrefix + errorPayload.Details.ErrorMessage);
                                break;
                            case Microsoft.WebPortal.ErrorCode.DownstreamServiceError:
                                environmentNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.DownstreamErrorPrefix + errorPayload.Details.ErrorMessage);
                                break;
                            default:
                                environmentNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.CustomerRegistrationFailureMessage);
                                break;
                        }
                    } else {
                        environmentNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.CustomerRegistrationFailureMessage);
                    }
                })
                .always(function () {
                    self.isPosting = false;
                });
        }
    };

    this.getEnvironmentInformation = function () {
        var environmentInformation = {
            Endpoint: this.environmentProfileView.viewModel.Endpoint(),
            Name: this.environmentProfileView.viewModel.Name(),
            Username: this.environmentProfileView.viewModel.Username(),
            Password: this.environmentProfileView.viewModel.Password(),
        };

        return environmentInformation;
    };
};

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.EnvironmentAddNewPagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.EnvironmentAddNewPagePresenter.prototype.onActivate = function () {
    /// <summary>
    /// Called when the presenter is activated.
    /// </summary>
};

Microsoft.WebPortal.EnvironmentAddNewPagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is about to be rendered.
    /// </summary>

    ko.applyBindings(this, $("#Form")[0]);

    this.environmentProfileView.render();
};

Microsoft.WebPortal.EnvironmentAddNewPagePresenter.prototype.onShow = function () {
    /// <summary>
    /// Called when content is shown.
    /// </summary>

    this.environmentProfileView.show();
};

//@ sourceURL=EnvironmentAddNewPagePresenter.js