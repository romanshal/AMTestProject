import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Recclass } from '../../models/recclass';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RecclassService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/v1/recclasses`;

  constructor(private http: HttpClient) { }

  getRecclasses(): Observable<Recclass[]> {
    return this.http.get<Recclass[]>(this.apiUrl);
  }
}
