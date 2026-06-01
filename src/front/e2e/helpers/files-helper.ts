import path from 'path';
import { Locator, Page } from 'playwright/test';

export async function uploadFiles(page: Page, fileUploadButton: Locator, files: string[]): Promise<void> {
  const [fileChooser] = await Promise.all([page.waitForEvent('filechooser'), fileUploadButton.click()]);

  await fileChooser.setFiles(files.map((f) => path.join(__dirname, `../assets/${f}`)));
}
