import { Component } from '@angular/core';
import { MeteoriteFilters, SortBy, SortOrder } from '../common/models/filter';
import { Sort } from '@angular/material/sort';
import { MeteoriteService } from '../common/services/meteorite/meteorite.service';
import { MeteoriteGroup } from '../common/models/meteorite';
import { catchError, finalize, of } from 'rxjs';
import { NotificationService } from '../common/notification/notification.service';

@Component({
  selector: 'app-catalog',
  standalone: false,
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss'
})
export class CatalogComponent {
  data: MeteoriteGroup[] = [];

  filter: MeteoriteFilters = {};

  isLoading = false;

  constructor(
    private service: MeteoriteService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;

    this.service.getGrouped(this.filter)
      .pipe(
        catchError(err => {
          this.notificationService.error('Error loading meteorite data.')
          return of([] as MeteoriteGroup[]);
        }),
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(data => {
        this.data = data;
      });
  }

  onSearch(filters: MeteoriteFilters) {
    this.filter = filters;

    this.loadData();
  }

  onSortChange(sort: Sort) {
    const sortBy: SortBy = (sort.active as SortBy) ?? 'year';
    const sortOrder: SortOrder = sort.direction === 'asc' || sort.direction === 'desc'
      ? sort.direction
      : 'desc';

    this.filter = {
      ...this.filter,
      sortBy,
      sortOrder
    };

    this.loadData();
  }
}
