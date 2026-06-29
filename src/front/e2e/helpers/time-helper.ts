import {
  APIRequestContext,
  Page,
  PlaywrightTestArgs,
  PlaywrightTestOptions,
  PlaywrightWorkerArgs,
  PlaywrightWorkerOptions,
  TestType,
} from '@playwright/test';

export async function getCurrentTime(request: APIRequestContext) {
  const apiUrl = process.env.API_URL ?? 'http://localhost:44210';
  const res = await request.get(`${apiUrl}/info/date`);
  const dateStr: string = await res.json();
  return new Date(dateStr);
}

// This will be called when tests need to be run with the browser time set to the test value of 2024-01-01T10:00:00
export function setTestTime(
  test: TestType<PlaywrightTestArgs & PlaywrightTestOptions, PlaywrightWorkerArgs & PlaywrightWorkerOptions>
): void {
  test.beforeEach(async ({ page }) => {
    await setFixedTime(page);
  });
}

export async function setFixedTime(page: Page) {
  const currentTime = await getCurrentTime(page.request);
  await page.clock.install({ time: currentTime });
}
