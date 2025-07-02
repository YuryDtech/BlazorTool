# Changelog

## 2025-07-02
- **UI and Functionality:**
    - Users can now click on the "Actions" statistic in the work order view to see a detailed list of activities.
    - The activity list is displayed in a compact, readable table format.
- **Code Changes:**
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