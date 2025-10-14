import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Recclass } from '../../models/recclass';
import { RecclassService } from '../../services/recclass/recclass.service';
import { catchError, finalize, map, Observable, of, startWith, switchMap, tap } from 'rxjs';
import { MeteoriteFilters } from '../../models/filter';
import { NotificationService } from '../../notification/notification.service';
import { MatDatepicker } from '@angular/material/datepicker';
import { maxYearValidator, minYearValidator } from '../validators/form-validators';
import { yearRangeValidator } from '../validators/year-range-validator';

export const YEAR_ONLY_FORMATS = {
  parse: {
    dateInput: 'YYYY',
  },
  display: {
    dateInput: 'YYYY',
    monthYearLabel: 'YYYY',
    dateA11yLabel: 'YYYY',
    monthYearA11yLabel: 'YYYY',
  },
};

@Component({
  selector: 'app-meteorite-filters',
  standalone: false,
  templateUrl: './meteorite-filters.component.html',
  styleUrl: './meteorite-filters.component.scss'
})
export class MeteoriteFiltersComponent implements OnInit {
  filterForm: FormGroup;
  recclasses: Recclass[] = [];
  filteredRecclasses!: Observable<Recclass[]>;

  isLoading = false;

  @Output() search = new EventEmitter<any>();

  constructor(
    private fb: FormBuilder,
    private recclassService: RecclassService,
    private notificationService: NotificationService

  ) {
    this.filterForm = this.fb.group({
      yearFrom: [null, [minYearValidator(0), maxYearValidator(new Date().getFullYear())]],
      yearTo: [null, [minYearValidator(0), maxYearValidator(new Date().getFullYear())]],
      recclass: [null],
      name: ['', [Validators.maxLength(50)]]
    },
      { validators: yearRangeValidator });
  }

  ngOnInit(): void {
    this.loadRecclasses();
  }

  private loadRecclasses(): void {
    this.isLoading = true;
    this.filteredRecclasses = this.recclassService.getRecclasses()
      .pipe(
        tap(data => {
          this.recclasses = data;
        }),
        catchError(err => {
          this.notificationService.error('Error loading recclass data.')
          console.error(err);

          return of([] as Recclass[]);
        }),
        switchMap(() =>
          this.filterForm.get('recclass')!.valueChanges.pipe(
            startWith(''),
            map(value => typeof value === 'string' ? value : value?.className),
            map(name => name ? this._filter(name) : this.recclasses.slice())
          )
        ),
        finalize(() => {
          this.isLoading = false;
        })
      );
  }

  chosenYearHandler(normalizedYear: Date, datepicker: MatDatepicker<Date>, controlName: 'yearFrom' | 'yearTo') {
    const year = normalizedYear.getFullYear();
    this.filterForm.get(controlName)?.setValue(new Date(year, 0, 1));
    datepicker.close();
  }

  private _filter(value: string): Recclass[] {
    const filterValue = value.toLowerCase();

    return this.recclasses.filter(recclass => recclass.className.toLowerCase().includes(filterValue));
  }

  displayFn = (rec: Recclass | string | null): string =>
    typeof rec === 'string'
      ? rec
      : rec?.className ?? '';

  onSearch() {
    const raw = this.filterForm.value;

    const clean: MeteoriteFilters = {
      yearFrom: raw.yearFrom instanceof Date ? raw.yearFrom.getFullYear() : undefined,
      yearTo: raw.yearTo instanceof Date ? raw.yearTo.getFullYear() : undefined,
      recclass: raw.recclass?.classId ?? undefined,
      name: raw.name?.trim() || undefined
    };

    this.search.emit(clean);
  }

  onReset() {
    this.filterForm.reset();
    this.search.emit({});
  }
}
