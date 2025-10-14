import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MeteoriteGroup } from '../../models/meteorite';
import { MeteoriteFilters, SortBy, SortOrder } from '../../models/filter';

@Injectable({
  providedIn: 'root'
})
export class MeteoriteService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/v1/meteorites`;

  constructor(private http: HttpClient) { }

  getGrouped(filters: MeteoriteFilters): Observable<MeteoriteGroup[]> {
    let params = new HttpParams();

    if (filters.yearFrom !== undefined) {
      params = params.set('yearFrom', filters.yearFrom.toString());
    }
    if (filters.yearTo !== undefined) {
      params = params.set('yearTo', filters.yearTo.toString());
    }
    if (filters.recclass !== undefined) {
      params = params.set('recclass', filters.recclass);
    }
    if (filters.name) {
      params = params.set('name', filters.name);
    }
    const sortBy: SortBy = filters.sortBy ?? 'year';
    const sortOrder: SortOrder = filters.sortOrder ?? 'desc';

    params = params.set('sortBy', sortBy);
    params = params.set('sortOrder', sortOrder);

    return this.http.get<MeteoriteGroup[]>(this.apiUrl, { params });
  }
}
