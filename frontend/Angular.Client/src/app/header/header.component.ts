import { Component, Input } from '@angular/core';

export interface NavItem {
  label: string;
  route: string;
}

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  @Input() navItems: NavItem[] = [];
}
