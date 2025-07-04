# Changelog

## 2025-07-04
- **UI and Functionality:**
    - Implemented user authorization via `Login.razor` page.
    - Integrated `UserState` for managing current user's login and token.
- **Code Changes:**
    - Added `Password` property to `UserState.cs`.
    - Refactored `AuthHeaderHandler.cs` to retrieve user credentials from `UserState` for token refresh.
    - Modified `BlazorTool.Client/Program.cs` to correctly inject `UserState` into `AuthHeaderHandler`.
    - Added `PostSingleAsync` method to `ApiServiceClient.cs` for handling single object API responses.
    - Updated `Login.razor` to use `ApiServiceClient.PostSingleAsync` for authentication and to update `UserState` upon successful login.
    - Completed `RightMatrix` model to correctly deserialize user rights.
    - Modified `IdentityController.cs` to return full `IdentityData` from external API.

## 2025-07-04
- **Code Changes:**
    - Added `Expires` property to `IdentityData` model.
    - Refactored `AuthHeaderHandler` to use `IdentityData` and `ApiResponse<IdentityData>` for token management.

## 2025-07-03
- **UI and Functionality:**
    - Implemented the creation of new appointments in the Scheduler.
    - Added a combobox for selecting a Device when creating a new appointment.
    - Implemented validation for required fields in the AppointmentEditor.
    - Added a popup notification to alert users about unfilled required fields.
    - Renamed the "Title" field to "AssetNo" in the AppointmentEditor and highlighted it if empty.
    - Improved `ChangelogPage` to render Markdown for better readability.
- **Code Changes:**
    - Added logic to `AppointmentService` to handle the creation of new appointments.
    - Implemented saving new appointments via `ApiServiceClient`.
    - Added a dropdown list for `Device` selection in `WorkOrderComponent` for new appointments.
    - Added validation logic in `AppointmentEditor.razor`.
    - Implemented a Telerik Popup for notifications in `AppointmentEditor.razor`.
    - Updated styles in `AppointmentEditor.razor.css`.
    - Added `Markdig` package to render Markdown in `ChangelogPage`.

## 2025-07-02
- **UI and Functionality:**
    - Added a status filter to the `OrdersPage`.
    - Improved filtering logic on the `OrdersPage`.
    - Added descriptive labels for the filters.
    - Added total workload to the activity list header.
    - The `WorkOrderComponent` now expands to show the full list of activities without scrolling.
    - Aligned the column sizes between the header in `ActivityList` and the items in `ActivityDisplay` for a consistent look.
    - Users can now click on the "Actions" statistic in the work order view to see a detailed list of activities.
    - The activity list is displayed in a compact, readable table format.
- **Code Changes:**
    - Added `totalWorkload` calculation in `ActivityList.razor`.
    - Used a conditional CSS class in `WorkOrderComponent.razor` to adjust `max-height` when activities are visible.
    - Corrected JSON deserialization for the `Activity` model by adding `[JsonConstructor]` to the default constructor.
    - Created `Activity.cs` model.
    - Added `ActivityController` to fetch activity data from the external API.
    - Implemented `GetActivityByWO` in `ApiServiceClient` to retrieve activities.
    - Developed `ActivityList` and `ActivityDisplay` Blazor components to show the activities.
    - Modified `WorkOrderComponent` to display the activity list on user interaction.
    - Beautify ChangelogPage
## 2025-07-01
- **UI and Functionality:**
    - On `SchedulerPage`, added a `Device` filter by `AssetNo` after the `Department` filter.
    - Made `WorkOrderComponent` editable, adding a Telerik ComboBox for the `WOCategory`, `WOLevel` fields.
    - In `WorkOrderComponent`, highlighted empty `Department` and `StartDate` fields.
    - On `SchedulerPage`, changed the logic for displaying taken/untaken orders.
    - On `SchedulerPage`, customized the scheduler item's text and color based on its state.
- **Code Changes:**
    - Added a new endpoint `api/v1/wo/getdict` to the `WoController` to retrieve work order categories.
    - Updated `ApiServiceClient` to fetch `Dict` objects.
    - Added `Dict.cs` model.
- **Bug Fixes:**
    - Fixed an issue in `WorkOrderComponent` where `Department` and `assignedPersons` were not displaying correctly.

## 2025-06-30
- **UI and Functionality:**
    - Made `WorkOrderComponent` editable, adding Telerik components for fields `assignedPersons`, `Department`, and `Description`.
    - On `OrderPage`, added a 'Device' column.
    - On `SchedulerPage`, added `OnClick` functionality for orders (to view WorkOrders in the order list) and implemented header filters for orders.
- **Code Changes:**
    - made and added `telerik_manual.md` for Telerik component usage instructions.

## 2025-06-27
- Added Changelog page and updated logging.
- Introduced `ChangelogPage.razor` and `CHANGELOG.md` for release notes.
- Replaced `Debug.Print` with `Console.WriteLine` for consistent logging across client and server.
- Minor UI adjustments in `MainLayout.razor`, `OrdersPage.razor`.
- Configured `CHANGELOG.md` to be copied to `wwwroot` for client-side access.
- Implemented display of `CHANGELOG.md` content on `ChangelogPage.razor`.
- Configured default `HttpClient` in `Program.cs` with `BaseAddress` for static file access.
- Corrected `Link` attribute in `BlazorTool.Client.csproj` for `CHANGELOG.md` to ensure correct placement in `wwwroot`.
- Reverted `app.UseBlazorFrameworkFiles()` in `BlazorTool/Program.cs` as it caused application loading issues.
- Re-added `HttpClient` registration with `BaseAddress` and `serverBaseUrl` check in `BlazorTool.Client/Program.cs`.
- **SchedulerPage.razor:**
    - Added device category filter to the untaken orders grid.
    - Implemented a checkbox to show/hide completed orders in the untaken orders grid.
    - Added `OnChange` event for assigned persons multiselect to trigger immediate display updates.
    - Adjusted layout for filter controls and order display.

## 2025-06-26
- Enhanced Scheduler UI and updated Blazor render mode.
- Configured client HttpClient for dynamic server base URL.
- Gitignore updated.
- Implemented PersonController and refactored API client for external calls.

## 2025-06-25
- In process: adding Filters to Scheduler.
- WorkOrderComponent: added scrolling, more compact data view, dynamic status color. OrdersPage: aligned filter items inline.

## 2025-06-24
- Added filters to Orders Page, including multiSelect.
- Fixed AUTH problems, added different http clients.
- Refactored Program.cs: DI, added AuthHeaderHandler for getting token, removed getting token for ApiServiceClient, added HostAddress in appSettings for server.
- Aligned SchedulerEditor buttons.

## 2025-06-23
- Implemented AppointmentService Save/Delete, ApiServiceClient Save/DeleteAppointment.
- Added Appointment view in Scheduler, and Scheduler - Move/Update items.
- Updated SchedulerAppointment to inherit from WorkOrder.
- Made WorkOrderComponent view only.

## 2025-06-20
- Configured OrdersPage with 3 columns.
- Enabled opening orders on click.
- Implemented API check to Server, Save and Read settings.
- Added Settings controller.
- Added Settings page, TestApiAddress settings.razor, and CheckApiAddress in ApiServiceClient.
- Added Order page.