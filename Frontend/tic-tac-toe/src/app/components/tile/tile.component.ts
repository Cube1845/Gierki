import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Move } from '../../models/move';
import { Position } from '../../models/position';
import { BoardService } from '../../services/board.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-tile',
  standalone: true,
  imports: [],
  templateUrl: './tile.component.html',
  styleUrl: './tile.component.scss',
})
export class TileComponent {
  @Input({ required: true }) x!: number;
  @Input({ required: true }) y!: number;
  @Input({ required: true }) value!: string;
  @Input({ required: true }) isDisabled!: boolean;

  @Output() tileClick = new EventEmitter<Position>();

  constructor() {}

  onTileClick(): void {
    var position = { 
      x: this.x, 
      y: this.y 
    };
    this.tileClick.emit(position);
  }
}
