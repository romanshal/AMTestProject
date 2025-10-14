import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Sort } from '@angular/material/sort';
import { MeteoriteGroup } from '../../models/meteorite';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-meteorite-table',
  standalone: false,
  templateUrl: './meteorite-table.component.html',
  styleUrl: './meteorite-table.component.scss'
})
export class MeteoriteTableComponent {
  @Input() data: MeteoriteGroup[] = [];
  @Output() sortChange = new EventEmitter<Sort>();

  displayedColumns = ['year', 'count', 'mass',];
  noDataColumns: string[] = ['noData'];

  get totalCount(): number {
    return this.data.reduce((acc, row) => acc + row.count, 0);
  }

  get totalMass(): number {
    return this.data.reduce((acc, row) => acc + row.totalMass, 0);
  }
}
