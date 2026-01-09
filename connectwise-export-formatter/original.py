import json
import os
import re
import logging
from datetime import datetime
from reportlab.lib import colors
from reportlab.lib.pagesizes import LETTER
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, Image

# --- Configuration ---
INPUT_FILE = __import__("sys").argv[1]  
OUTPUT_DIR = __import__("sys").argv[2]
LOGO_PATH = os.path.join(os.path.dirname(__file__), "logo.png")

# --- Setup Logging ---
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger()

def clean_filename(text):
    if not text: return "Untitled"
    clean = re.sub(r'[^\w\s-]', '', str(text))
    return clean.strip()[:100]

def clean_text(text):
    if text is None: return ""
    text = str(text)
    return text.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;').replace('\n', '<br/>')

def get_safe_val(record, *keys):
    """Deep search for a value. Returns string."""
    val = record
    for k in keys:
        if isinstance(val, dict):
            val = val.get(k)
        else:
            return ""
    return str(val) if val is not None else ""

def is_valid_record(data):
    """
    Checks if the JSON object is a 'real' record or just a fragment/garbage.
    Returns True if it has enough info to be worth printing.
    """
    if not isinstance(data, dict):
        return False
    
    # If it's just {}, it's invalid
    if not data:
        return False

    # Check for critical fields usually present in ConnectWise/IT Glue exports
    has_name = 'name' in data and data['name']
    has_company = 'company' in data
    has_questions = 'questions' in data and len(data['questions']) > 0

    if has_name or (has_company and has_questions):
        return True
    
    return False

def generate_pdf(record, filename, title_header):
    try:
        doc = SimpleDocTemplate(filename, pagesize=LETTER, rightMargin=72, leftMargin=72, topMargin=72, bottomMargin=72)
        story = []
        styles = getSampleStyleSheet()

        # Custom Styles
        title_style = styles['Heading1']
        subtitle_style = styles['Heading3']
        label_style = ParagraphStyle('LabelStyle', parent=styles['Normal'], fontName='Courier-Bold', spaceAfter=2)
        normal_style = ParagraphStyle('NormalCourier', parent=styles['Normal'], fontName='Courier')

        # Add Logo at the top if it exists
        if os.path.exists(LOGO_PATH):
            try:
                logo = Image(LOGO_PATH)
                # Scale logo to fit nicely (2 inches wide, maintain aspect ratio)
                logo.drawHeight = 0.75*inch
                logo.drawWidth = 2.0*inch
                # Center the logo
                logo.hAlign = 'CENTER'
                story.append(logo)
                story.append(Spacer(1, 15))
            except Exception as e:
                logger.warning(f"Could not load logo: {e}")

        # Header
        display_title = title_header if title_header else record.get('name', 'Configuration Report')
        story.append(Paragraph(clean_text(display_title), title_style))
        story.append(Spacer(1, 10))

        # Meta Data
        meta_data = [
            ["Name:", clean_text(record.get('name', ''))],
            ["Company:", clean_text(get_safe_val(record, 'company', 'name'))],
            ["Site:", clean_text(get_safe_val(record, 'site', 'name'))],
            ["Type:", clean_text(get_safe_val(record, 'type', 'name'))],
            ["Status:", clean_text(get_safe_val(record, 'status', 'name'))]
        ]
        
        # Remove empty rows to save space
        meta_data = [row for row in meta_data if row[1]]

        if meta_data:
            formatted_meta = [[Paragraph(m[0], label_style), Paragraph(m[1], normal_style)] for m in meta_data]
            t_meta = Table(formatted_meta, colWidths=[1.5*inch, 4.5*inch])
            t_meta.setStyle(TableStyle([
                ('VALIGN', (0,0), (-1,-1), 'TOP'),
                ('LINEBELOW', (0,0), (-1,-1), 0.5, colors.lightgrey),
            ]))
            story.append(t_meta)
            story.append(Spacer(1, 20))

        # Notes
        notes = record.get('notes', '')
        vendor = record.get('vendorNotes', '')
        if notes or vendor:
            story.append(Paragraph("Notes", subtitle_style))
            if notes:
                story.append(Paragraph("<b>General:</b> " + clean_text(notes), normal_style))
                story.append(Spacer(1, 5))
            if vendor:
                story.append(Paragraph("<b>Vendor:</b> " + clean_text(vendor), normal_style))
            story.append(Spacer(1, 15))

        # Questions Table
        questions = record.get('questions', [])
        valid_qa = []
        if isinstance(questions, list):
            for q in questions:
                if isinstance(q, dict):
                    q_txt = str(q.get('question', '')).strip()
                    a_txt = str(q.get('answer', '')).strip()
                    if q_txt:
                        valid_qa.append([q_txt, a_txt])

        if valid_qa:
            story.append(Paragraph("Details", subtitle_style))
            table_data = [['Setting', 'Value']] + [[Paragraph(clean_text(r[0]), label_style), Paragraph(clean_text(r[1]), normal_style)] for r in valid_qa]
            
            t_qa = Table(table_data, colWidths=[3*inch, 3*inch], repeatRows=1)
            t_qa.setStyle(TableStyle([
                ('BACKGROUND', (0,0), (-1,0), colors.HexColor('#e0e0e0')),
                ('GRID', (0,0), (-1,-1), 0.5, colors.grey),
                ('VALIGN', (0,0), (-1,-1), 'TOP'),
                ('TOPPADDING', (0,0), (-1,-1), 4),
                ('BOTTOMPADDING', (0,0), (-1,-1), 4),
            ]))
            story.append(t_qa)

        doc.build(story)
        return True
    except Exception as e:
        logger.error(f"PDF Gen failed for {filename}: {e}")
        return False

def count_braces_ignoring_strings(line):
    """
    Counts { and } but ignores any that appear inside double quotes.
    Returns (net_change, has_open_brace)
    """
    # Remove escaped quotes first: \"
    clean = line.replace('\\"', '')
    # Remove quoted strings entirely: "..."
    # This regex finds " followed by anything that isn't ", followed by "
    clean = re.sub(r'"[^"]*"', '', clean)
    
    open_count = clean.count('{')
    close_count = clean.count('}')
    return open_count - close_count, '{' in clean

def process_file(filepath):
    if not os.path.exists(OUTPUT_DIR):
        os.makedirs(OUTPUT_DIR)

    logger.info(f"Starting processing of {filepath}...")
    
    buffer = []
    brace_balance = 0
    in_json = False
    potential_title = "Untitled"
    
    records_created = 0
    lines_read = 0

    with open(filepath, 'r', encoding='utf-8', errors='replace') as f:
        for line in f:
            lines_read += 1
            stripped = line.strip()
            
            if not stripped:
                continue

            # Calculate brace change for this line safely
            net_change, has_open = count_braces_ignoring_strings(line)

            # Case 1: We are not currently building a JSON object
            if not in_json:
                # If we see an opening brace, start recording
                if has_open:
                    in_json = True
                    brace_balance = 0 # Reset
                    buffer = [] 
                    
                    # Add line to buffer
                    buffer.append(line)
                    brace_balance += net_change
                else:
                    # It's a header/title line
                    # We only update the title if it looks like a real title (no braces, shortish)
                    if '{' not in stripped and '}' not in stripped and len(stripped) < 150:
                        potential_title = stripped
                        # logger.debug(f"Line {lines_read}: Found header '{potential_title}'")
            
            # Case 2: We ARE inside a JSON object
            else:
                buffer.append(line)
                brace_balance += net_change

                # Check if object is closed
                if brace_balance <= 0:
                    in_json = False
                    
                    # Attempt parse
                    full_str = "".join(buffer).strip()
                    # Fix trailing commas common in dumps
                    if full_str.endswith(','): full_str = full_str[:-1]

                    try:
                        data = json.loads(full_str)
                        
                        # VALIDATION: Is this a real record?
                        if is_valid_record(data):
                            # Generate Filename
                            safe_name = clean_filename(potential_title)
                            fname = f"{safe_name}.pdf"
                            
                            # Handle Duplicates
                            count = 1
                            while os.path.exists(os.path.join(OUTPUT_DIR, fname)):
                                fname = f"{safe_name}_{count}.pdf"
                                count += 1
                                
                            out_path = os.path.join(OUTPUT_DIR, fname)
                            
                            if generate_pdf(data, out_path, potential_title):
                                records_created += 1
                                if records_created % 10 == 0:
                                    logger.info(f"Created {records_created} PDFs (at line {lines_read})")
                        else:
                            #It was valid JSON, but empty or garbage (like 'customFields')
                            logger.warning(f"Line {lines_read}: Skipped valid but empty/fragment JSON.")
                            pass
                            
                    except json.JSONDecodeError as e:
                        logger.error(f"Line {lines_read}: JSON Parse Error. \nContext: {full_str[:50]}... \nError: {e}")
                    
                    # Reset
                    potential_title = "Untitled"

    logger.info(f"DONE. Processed {lines_read} lines. Generated {records_created} PDF files.")

if __name__ == "__main__":
    if os.path.exists(INPUT_FILE):
        process_file(INPUT_FILE)
    else:
        logger.error(f"File {INPUT_FILE} does not exist.")