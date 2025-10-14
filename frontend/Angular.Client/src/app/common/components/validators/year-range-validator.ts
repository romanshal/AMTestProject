import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export const yearRangeValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const yearFrom: Date | null = control.get('yearFrom')?.value;
  const yearTo: Date | null = control.get('yearTo')?.value;

  if (yearFrom && yearTo) {
    const fromYear = yearFrom.getFullYear();
    const toYear = yearTo.getFullYear();

    if (fromYear > toYear) {
      return { yearRange: { from: fromYear, to: toYear } };
    }
  }

  return null;
};