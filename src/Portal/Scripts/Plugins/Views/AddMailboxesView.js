Microsoft.WebPortal.Views.AddMailboxesView = function (webPortal, elementSelector, context, isShown, animation) {
    /// <summary>
    /// A view that renders UX showing a list of mailboxes to be added from a list.
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="elementSelector">The JQuery selector for the HTML element this view will own.</param>
    /// <param name="context">Contextual information to utilize for the view.</param>
    /// <param name="isShown">The initial show state. Optional. Default is false.</param>
    /// <param name="animation">Optional animation to use for showing and hiding the view.</param>

    this.base.constructor.call(this, webPortal, elementSelector, isShown, null, animation);
    this.template = "addMailboxes-template";

    this.viewModel = {
        EnvironmentId: context.Id
    };

    this.mailboxesList = new Microsoft.WebPortal.Views.List(this.webPortal, elementSelector + " #MailboxesList", this);

    this.mailboxesList.setColumns([
        new Microsoft.WebPortal.Views.List.Column("Name", null, false, false, null, null, null, "mailboxEntry-template")
    ]);

    this.mailboxesList.showHeader(false);
    this.mailboxesList.setEmptyListUI(this.webPortal.Resources.Strings.Plugins.AddMailboxesView.EmptyListCaption);
    this.mailboxesList.enableStatusBar(false);
    this.mailboxesList.setSelectionMode(Microsoft.WebPortal.Views.List.SelectionMode.None);

    this.AddMailboxToView = function (mailbox) {
        this.mailboxesList.append([{
            mailbox: mailbox
        }]);

        $(elementSelector + " #MailboxesList").height($(elementSelector + " #MailboxesList table").height());
        webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResizing);
        webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResized);
    };
};

// extend the base view
$WebPortal.Helpers.inherit(Microsoft.WebPortal.Views.AddMailboxesView, Microsoft.WebPortal.Core.View);

Microsoft.WebPortal.Views.AddMailboxesView.prototype.onRender = function () {
    /// <summary>
    /// Called when the view is rendered.
    /// </summary>

    $(this.elementSelector).attr("data-bind", "template: { name: '" + this.template + "'}");
    ko.applyBindings(this, $(this.elementSelector)[0]);
};

Microsoft.WebPortal.Views.AddMailboxesView.prototype.onShowing = function (isShowing) {
    /// <summary>
    /// Called when the view is about to be shown or hidden.
    /// </summary>
    /// <param name="isShowing">true if showing, false if hiding.</param>

    if (isShowing) {
        this.mailboxesList.show();
    }
    else {
        this.mailboxesList.hide();
    }
};

Microsoft.WebPortal.Views.AddMailboxesView.prototype.onShown = function (isShown) {
    /// <summary>
    /// Called when the view is shown or hidden.
    /// </summary>
    /// <param name="isShown">true if shown, false if hidden.</param>

    if (isShown) {
        // resize the list to fit its content
        $(this.elementSelector + " #MailboxesList").height($(this.elementSelector + " #MailboxesList table").height());

        // force a window resize for the list to resize
        this.webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResizing);
        this.webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResized);
    }
};

Microsoft.WebPortal.Views.AddMailboxesView.prototype.onDestroy = function () {
    /// <summary>
    /// Called when the view is about to be destroyed.
    /// </summary>

    if (this.mailboxesList) {
        this.mailboxesList.destroy();
    }

    if ($(this.elementSelector)[0]) {
        // if the element is there, clear its bindings and clean up its content
        ko.cleanNode($(this.elementSelector)[0]);
        $(this.elementSelector).empty();
    }
};

Microsoft.WebPortal.Views.AddMailboxesView.prototype.onAddMailboxClicked = function () {
    /// <summary>
    /// Called when the user wants to add mailboxes.
    /// </summary>

    var self = this;

    var okButton = Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, 1, function () {
        if (self.mailboxesDialogViewModel.mailboxList.getSelectedRows().length <= 0) {
            self.mailboxesDialogViewModel.errorMessage("SelectMailboxErrorMessage");
            return;
        }

        for (index = 0; index < self.mailboxesDialogViewModel.mailboxList.getSelectedRows().length; ++index) {
            self.AddMailboxToView(self.mailboxesDialogViewModel.mailboxList.getSelectedRows()[index]);
        }

        self.webPortal.Services.Dialog.hide();
    });

    var cancelButton = Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.CANCEL, 1, function () {
        self.webPortal.Services.Dialog.hide();
    });

    var getMailboxesServerCall =
        new Microsoft.WebPortal.Utilities.RetryableServerCall(
            this.webPortal.Helpers.ajaxCall("api/migrationbatch/mailboxes?environmentId=" + self.viewModel.EnvironmentId, Microsoft.WebPortal.HttpMethod.Get));

    getMailboxesServerCall.execute().done(function (value) {
        self.mailboxesDialogViewModel = {
            mailboxList: new Microsoft.WebPortal.Views.List(self.webPortal, "#mailboxList", self),
            errorMessage: ko.observable("")
        };

        self.mailboxesDialogViewModel.mailboxList.setColumns([
            new Microsoft.WebPortal.Views.List.Column("DisplayName", "min-width: 300px; width: 300px; white-space: normal;", true, false,
                "DisplayName"),
            new Microsoft.WebPortal.Views.List.Column("SamAccountName", "min-width: 100px; width: 100px;", true, false, "SamAccountName"),
            new Microsoft.WebPortal.Views.List.Column("UserPrincipalName", "min-width: 500px; white-space: normal;", false, false, "UserPrincipalName")
        ]);

        self.mailboxesDialogViewModel.mailboxList.setEmptyListUI("EmptyMailboxListMessage");
        self.mailboxesDialogViewModel.mailboxList.enableStatusBar(false);
        self.mailboxesDialogViewModel.mailboxList.setSelectionMode(Microsoft.WebPortal.Views.List.SelectionMode.Multiple);
        self.mailboxesDialogViewModel.mailboxList.setSorting("DisplayName", Microsoft.WebPortal.Views.List.SortDirection.Ascending, true);

        self.mailboxesDialogViewModel.mailboxList.set(value.Mailboxes);
        self.mailboxesDialogViewModel.mailboxList.setComplete(true);

        self.webPortal.EventSystem.subscribe(Microsoft.WebPortal.Event.DialogShown, self.onMailboxDialogShown, self);
        self.webPortal.Services.Dialog.show("mailboxPicker-template", self.mailboxesDialogViewModel, [okButton, cancelButton]);
        self.webPortal.Services.Dialog.showProgress();
    });
};

Microsoft.WebPortal.Views.AddMailboxesView.prototype.onMailboxDialogShown = function (eventId, isShown) {
    /// <summary>
    /// Called when the dialog box is shown or hidden.
    /// </summary>
    /// <param name="eventId">The event ID.</param>
    /// <param name="isShown">Indicates whether the dialog is shown or hidden.</param>

    if (isShown) {
        // show the list and hide the progress bar once the dialog is shown
        this.mailboxesDialogViewModel.mailboxList.show();
        this.webPortal.Services.Dialog.hideProgress();
    }

    // stop listening to dialog box events
    this.webPortal.EventSystem.unsubscribe(Microsoft.WebPortal.Event.DialogShown, this.mailboxesDialogViewModel, this);
};

//@ sourceURL=AddMailboxesView.js