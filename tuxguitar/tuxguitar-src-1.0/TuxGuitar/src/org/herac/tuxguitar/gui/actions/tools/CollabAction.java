/*
 * Created on 17-dic-2005
 *
 * TODO To change the template for this generated file go to
 * Window - Preferences - Java - Code Style - Code Templates
 */
package org.herac.tuxguitar.gui.actions.tools;

import org.eclipse.swt.events.TypedEvent;
import org.herac.tuxguitar.gui.TuxGuitar;
import org.herac.tuxguitar.gui.actions.Action;
import org.herac.tuxguitar.gui.tools.collab.CollabDialog;

/**
 * @author julian
 *
 * TODO To change the template for this generated type comment go to
 * Window - Preferences - Java - Code Style - Code Templates
 */
public class CollabAction extends Action{
	public static final String NAME = "action.tools.collab";
	
	public CollabAction() {
		super(NAME, AUTO_LOCK | AUTO_UNLOCK | AUTO_UPDATE);
	}
	
	protected int execute(TypedEvent e){
		// Aqui vou chamar a primeira janela da colaboração
		new CollabDialog().show();
		
		/*
		TuxGuitar.instance().getFretBoardEditor().setScaleChanges();
		TuxGuitar.instance().getPianoEditor().setScaleChanges(); */
		
		return 0;
	}
}
