# Quickstart — Document Upload and Management

## Run the application

1. Open a terminal in the repository root.
2. Change directory to the web app:

   ```powershell
   cd ContosoDashboard
   ```

3. Run the application:

   ```powershell
   dotnet run
   ```

4. Open the browser at `https://localhost:5001` or the URL shown in the Console.

5. Login via the existing mock login page.

## Verify the document management feature

1. Open the My Documents page from the app navigation.
2. Upload one or more supported files:
   - PDF, Word, Excel, PowerPoint, text, JPEG, PNG
3. Provide a title and category, then submit.
4. Confirm the newly uploaded document appears in the list with title, category, upload date, file size, and associated project.
5. Test preview and download for a supported file type.
6. Edit metadata on an owned document and save the update.
7. Delete a document and confirm both the metadata and stored file are removed.
8. Verify project document visibility from a project details page.
9. Share a document with another user and confirm the recipient receives an in-app notification.

## Notes for development

- Uploaded files should be stored outside `wwwroot`, for example in `AppData/uploads`.
- Use a `LocalFileStorageService` implementation for current storage and an `IFileStorageService` interface for future migration.
- Ensure authorization is enforced on all document download, preview, and management actions.
