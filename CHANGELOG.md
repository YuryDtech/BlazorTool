## 2025-07-30
### Added
- **Advanced Grid Filters**: Implemented advanced filtering capabilities in `TelerikGrid` on the `OrdersPage`, including filters for Department, Reason, Modified Person, Assigned Person, Work Order State, Work Order Level, Add Date, and Start Date.
- **Person View in Activities**: Added the ability to view assigned persons by name in `ActivityDisplay` and `ActivityList` components.
- **Person Data Caching**: Implemented caching for person data in `ApiServiceClient` to improve performance.
### Changed
- **UI/UX - Orders Page**: Old filters for Device Type and Order State were removed.
- **UI/UX - Work Order Status Colors**: Updated the display logic for workorder status colors to be based on `StateID` for consistency. Introduced new, lighter background colors for better readability.
### Fixed
- **Authentication Bug**:old user in cache. Modified the authentication header handling to clear user state upon login requests. 

## 2025-07-29
### Added
- **Scheduler Context Menu**: Implemented a right-click context menu in the Scheduler. Users can now:
    - Create new appointments by right-clicking on an empty time slot.
    - Open, remove, or duplicate existing appointments by right-clicking on them.	
### Changed
- **Settings**: Changing the API address on the Settings page now automatically reloads the application to ensure the new settings are applied correctly.

- **Color Scheme**: Refactored the status color logic to be based on State IDs, ensuring consistent color representation across the application (respecting the "Use original colors" setting). New, lighter background colors have been introduced for better readability.

### Fixed
- **Authentication**: Fixed an issue where a session check could incorrectly trigger on the login page, causing a redirect loop.
- **Appointment Editor**: Canceling the creation of a new appointment now correctly closes the editor window.

## 2025-07-28
### Added
- **Color Scheme Switcher**: Added a "Use original colors" checkbox in the navigation menu. This allows users to switch between the original colors provided by the API and a custom color palette.

### Changed
- **UI/UX**:
    - Replaced basic loading indicators with `TelerikLoaderContainer` on the `OrdersPage` and `SchedulerPage`.
    - The total order count is now displayed in the `OrdersPage` header.
- **Refactoring**:
    - Centralized color management into `AppStateService` to ensure a consistent color scheme across all components (`SchedulerSummary`, `OrdersPage`, `SchedulerPage`).
    - Components now subscribe to `AppStateService.OnChange` to dynamically update their appearance when the color scheme is changed.
- **API Address Change**: Implemented a mechanism to restart the application after changing the external API address on the Settings page. This ensures that the new address is correctly applied.
- **Confirmation for API Change**: Added a Telerik confirmation dialog on the Settings page, which appears when a user attempts to change the API address. The dialog displays both the old and new addresses and warns that the application will restart.
- **Refactoring**:
    - Replaced the obsolete `ListTypeEnum` with the correct `WOListTypeEnum` across the client-side application.
    - Improved data loading and filtering logic on the `SchedulerPage` for departments and device categories.
    - Refactored `ApiServiceClient` to correctly handle nullable `personID` in dictionary-related methods.

### Fixed
- **Error Handling**: Improved exception handling in `UserSettings.cs` to ensure errors are properly thrown and logged.

## 2025-07-24
### Changed
- **API Address Configuration**: The external API address can now be configured on the Settings page by the "MES" user. The address is stored globally for the application in the `appsettings.json` file on the server.
- **API Address Handling**: The application's server-side calls were unified to correctly handle API server addresses with or without the `/api/v1` suffix, using the centralized address from the configuration.

### Fixed
- **Scheduler Drag & Drop**: Fixed a critical bug where dragging a work order from the grid to the Scheduler would fail if the Scheduler was not in "Timeline" view. The assignment logic now correctly applies only to the Timeline view, preventing errors in other views.

## 2025-07-23
### Added
- **Build Date Display**: Implemented a mechanism to display the application's build date in the main layout, providing clear versioning information.
- **Scheduler Summary Table**: Added a summary table on the Scheduler page (Timeline view) that shows work order counts per person, broken down by status. This provides an at-a-glance overview of each team member's workload.
- **Orders Page Filtering**: Introduced a new filter for "Assigned persons" on the Orders page, allowing users to narrow down the work order list by specific team members.

### Fixed
- **Scheduler Drag & Drop**: Corrected an issue where dragging a work order from the grid to the Scheduler's timeline did not correctly assign the appointment to the target person (resource).
- **Filter UI Behavior**: The "Assigned persons" filter on the Orders page now updates the list instantly when cleared via the "Clear" button.
- **Filter Label Alignment**: Fixed the alignment of the "Assigned persons" filter label on the Orders page to ensure it is positioned correctly above the multiselect component, consistent with other filters.

## 2025-07-22
## Added
- **Scheduler Timeline View:** Introduced a new Timeline view for the scheduler, allowing for horizontal display of appointments.
- **Dynamic Timeline View Settings:** Added UI controls to dynamically adjust `Column Width`, `Slot Duration`, and `Slot Divisions` for the Timeline view, providing greater flexibility in how time intervals are displayed.
- **Scheduler Item Component:** Introduced a reusable `SchedulerItem` component to standardize the rendering of appointments across different scheduler views.
- **Scheduler Loading Indicator:** Added a loading indicator to the scheduler to provide visual feedback during data fetching and authorization processes.

## Changed
- **Scheduler Grouping Logic:** Resolved `KeyNotFoundException` in Timeline view grouping by ensuring the correct resource field (`AssignedPerson`) is referenced in `SchedulerGroupSettings`.
- **Scheduler UI Enhancements:** Applied custom styling to scheduler items and day cells in Month, Day, and Week views for improved visual appearance.
- **Cache Management Refactoring:** Refactored internal cache management for appointments, leading to more efficient data handling and smoother UI updates in the scheduler.
- **Appointment Service Method Rename:** Renamed `CloseAppointment` method to `RemoveAppointment` in the `AppointmentService` for clearer semantics.

## Fixed
- `KeyNotFoundException`: Addressed the `KeyNotFoundException` occurring when switching to the Telerik Scheduler Timeline view, specifically when grouping by assigned persons.


## 2025-07-21
- **Refactoring & Bug Fixes:**
    - Refactored cache management in `ApiServiceClient` for `WorkOrder` objects to ensure data consistency. Implemented centralized methods for updating, invalidating, and refreshing the cache (`UpdateWorkOrderInCache`, `InvalidateWorkOrdersCache`, `RefreshWorkOrderInCacheAsync`).
    - All `WorkOrder` manipulation methods (`Update`, `Save`, `Close`) now use the new centralized cache logic.
    - Corrected the UI update logic in `SchedulerPage.razor`. The local appointment collection (`_allAppointments`) is now correctly modified when an appointment is changed or closed, ensuring the UI reflects the current state.
    - Enhanced `AppointmentEditor.razor` to support closing work orders and properly propagate UI updates.
    - Added the `WorkOrderInfo.cs` model and updated `WoController` to support fetching detailed work order information.

## 2025-07-09
- **Functionality & UI:**
    - Implemented the complete lifecycle for Work Orders, allowing users to create, update, and close them through the API.
    - Added robust validation for required fields (like Description, Level) in the Work Order editor, with visual highlighting for missing data.
    - Closing a Work Order now requires at least one associated activity.
    - Pop-up notifications now display specific error messages from the API, providing clearer user feedback.
- **Technical & Refactoring:**
    - Implemented a server-side session check on application load. Users are now automatically logged out if their server session is invalid, preventing API errors.
    - Refactored `ApiServiceClient` methods (`Save`, `Update`, `Close`) to return a `SingleResponse<T>` object, providing detailed error information.
    - Added dedicated API endpoints and request models (`WorkOrderCreateRequest`, `UpdateWorkOrderRequest`, `CloseWorkOrderRequest`) for all Work Order operations.
    - Switched from using string names to IDs for entities like Categories and Levels, improving data integrity.

## 2025-07-08
- **Work Order Management Enhancement**: Implemented client-side validation and mapping of `WorkOrder` to `WorkOrderCreateRequest` in `ApiServiceClient.SaveWorkOrderAsync`. The `WoController.Create` endpoint now directly accepts `WorkOrderCreateRequest`. `ApiServiceClient.UpdateWorkOrderAsync` now utilizes `SaveWorkOrderAsync` for new work orders, ensuring consistent API saving. Added `ReasonID`, `CategoryID`, `DepartmentID`, and `AssignedPersonID` fields to `WorkOrderCreateRequest`.
- **Client-Server Token Synchronization**: Implemented a robust mechanism where the Blazor server retrieves external API tokens from its cache using the `PersonID` provided by the client in a custom `X-Person-ID` HTTP header. This eliminates the need for the client to directly send the external API token to the Blazor server.
- **Client-side Initialization Enhancement**: Ensured that `UserState` data is fully loaded from `localStorage` before any client-side API calls are made. This was achieved by awaiting `UserState.InitializationTask` in `AuthHeaderHandler` and relevant Razor pages (`SchedulerPage`, `OrdersPage`, `Settings`).
- **Server-side Token Management**: The `IdentityController` now caches `IdentityData` (including the external API token) upon successful user login. The `ServerAuthTokenService` has been refactored to retrieve these tokens from the server's `IMemoryCache` based on the `PersonID` extracted from the `X-Person-ID` header.
- **API Error Handling**: Added `try-catch` blocks to all HTTP request methods within `ApiServiceClient` to gracefully handle network and API errors, logging them to the console and returning empty/default values.
- **Dependency Refinement and Cleanup**: Removed obsolete JWT Bearer authentication configurations and hardcoded login credentials from the server-side `Program.cs`, streamlining the authentication and dependency injection setup.
- **API Documentation**: Added `API-info.md` to provide clear documentation of API endpoints and their authorization types.

## 2025-07-07
- **UI and Functionality:**
    - Improved login experience with a searchable dropdown for username and basic input validation.
    - Enhanced Work Order editor with dynamic selection for categories and levels, and improved device/category display.
    - Automatic caching of new dictionary entries (categories, levels, departments).
    - Refined work order filtering logic on the Scheduler page.
    - Improved API client error handling.
- **Code Changes:**
    - Removed `ListDicts` parameter from `AppointmentEditor.razor`.
    - Updated `WorkOrderComponent.razor` to use `ListCategories` and `ListLevels` for TelerikComboBox, removed `ListDicts` initialization logic, and added `selectedDevice` update logic.
    - Replaced direct `ListDicts` manipulations with `apiService.AddNewWODict` calls for categories, departments, and levels.
    - Replaced username input with `TelerikComboBox` in `Login.razor`.
    - Added loading indicator and removed `ListDicts` parameter from `SchedulerPage.razor`.
    - Modified work order filtering logic in `SchedulerPage.razor`.
    - Added `_dictCache` and new dictionary-related methods (`GetWODictionaries`, `GetWODictionariesCached`, `GetWOCategories`, `GetWOStates`, `GetWOLevels`, `GetWOReasons`, `GetWODepartments`, `AddNewWODict`) to `ApiServiceClient.cs`.
    - Improved error handling in `PostSingleAsync` in `ApiServiceClient.cs`.
    - Added a parameterless constructor to `UserState.cs`.
    - Changed login endpoint to `api/v1/identity/loginpass` and method to GET in `IdentityController.cs`.
    - Registered `UserState` as a Scoped service in `Program.cs`.

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
    - Added `Expires` property to `IdentityData` model.
    - Refactored `AuthHeaderHandler` to use `IdentityData` and `ApiResponse<IdentityData>` for token management.
    - Enhanced `UserState` to persist `IdentityData` to `localStorage` and load it on startup.
    - Modified `Login.razor` to explicitly save `IdentityData` to `UserState` (and thus `localStorage`) after successful login.

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