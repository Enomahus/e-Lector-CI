import { Locator, Page } from 'playwright/test';

export async function inputDate(
  page: Page,
  dateInput: Locator,
  day: string,
  month: string,
  year: string,
  hour?: string,
  minutes?: string
) {
  await dateInput.clear();
  await page.keyboard.press(day[0]);
  await page.keyboard.press(day[1]);

  await page.keyboard.press(month[0]);
  await page.keyboard.press(month[1]);

  await page.keyboard.press(year[0]);
  await page.keyboard.press(year[1]);
  await page.keyboard.press(year[2]);
  await page.keyboard.press(year[3]);

  if (hour !== undefined && minutes !== undefined) {
    await page.keyboard.press(hour[0]);
    await page.keyboard.press(hour[1]);
    await page.keyboard.press(minutes[0]);
    await page.keyboard.press(minutes[1]);
  }
}
